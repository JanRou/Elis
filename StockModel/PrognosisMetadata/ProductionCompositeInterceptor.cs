using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ED.Wp3.Server.BE.PrognosisMetadata.Model;

namespace ED.Wp3.Server.BE.PrognosisMetadata
{
    public class ProductionCompositeInterceptor : Composite
    {


        private readonly IPrognosisMetadataProvider _prognosisMetadataProvider;
        private readonly IControlCodeInterpreter _controlCodeInterpreter;

        public ProductionCompositeInterceptor(string name, IPrognosisMetadataProvider prognosisMetadataProvider, IControlCodeInterpreter controlCodeInterpreter) : base(name)
        {

            _prognosisMetadataProvider = prognosisMetadataProvider;
            _controlCodeInterpreter = controlCodeInterpreter;
            _type = ComponentType.CompoInterceptor;
        }
        public override void Create()
        {
            Clean();
            // Create new production composite from database
            foreach (PrognosisMetadataProductionDto dto in _prognosisMetadataProvider.GetPrognosisProduction() )
            {
                IComposite prognosis =  GetOrAddComposite(dto.ModelId, this); 
                AddContentToPrognosis( prognosis, dto);
            }
        }

        private void AddContentToPrognosis(IComposite prognosis, PrognosisMetadataProductionDto dto)
        {
            AddLeaf<string>(prognosis, "Id", dto.ModelId);
            AddLeaf<string>(prognosis, "Name", dto.ModelName);
            AddLeaf<string>(prognosis, "Description", dto.ModelDescription);
            IComposite windArea = GetOrAddComposite("WindArea", prognosis);
            AddWindArea(windArea, dto);
        }
        private void AddWindArea(IComposite windArea, PrognosisMetadataProductionDto dto)
        {
            IComposite windAreaSub = GetOrAddComposite(dto.WindAreaId, windArea);
            AddLeaf<string>(windAreaSub, "Id", dto.WindAreaName);
            AddLeaf<string>(windAreaSub, "Name", dto.WindAreaName);
            IComposite status = GetOrAddComposite("Status", windAreaSub);
            AddWindAreaStatus(status, dto);
        }

        private void AddWindAreaStatus(IComposite status, PrognosisMetadataProductionDto dto)
        {
            ControlCodeDescriptor ccd = _controlCodeInterpreter.Convert(dto.ControlCode, dto.ControlValue);
            IComposite category = GetOrAddComposite(ccd.RunCategory, status);
            IComposite code = GetOrAddComposite(ccd.Code.ToString(), category);
            AddStatusCode(code, ccd, dto.Updated);
        }

        private void AddStatusCode(IComposite code, ControlCodeDescriptor ccd, DateTime updated)
        {
            AddLeaf<string>(code, "Id", ccd.Code.ToString());
            AddLeaf<string>(code, "Name", ccd.Name);
            AddLeaf(code, "Value", ccd.Value, ccd.Type);
            AddLeaf<string>(code, "Type", ccd.Type.ToString() );
            AddLeaf<DateTime>(code, "Updated", updated);
        }

        private IComposite GetOrAddComposite(string name, IComposite composite)
        {
            IComposite subComposite = (IComposite) composite.Get(name);
            if ( subComposite == null )
            {
                subComposite = new Composite(name);
                composite.Add(subComposite);
            }
            return subComposite;
        }

        private void Clean()
        {
            foreach (string name in this.Select(component => component.Name).ToList())
            { 
                Remove(name);   
            }
        }
        private void AddLeaf<T>(IComposite composite, string fieldId,  T value)
        {
            if ( ( value != null ) && ( composite.Get(fieldId) == null ) )
            {
                // add missing field
                composite.Add(new Leaf<T>(fieldId) { Value = value });
            }
        }
        private void AddLeaf(IComposite composite, string fieldId, object value, Type type)
        {
            if ( ( value != null ) && ( composite.Get(fieldId) == null ) )
            {
                if ( type == typeof(bool) )
                {
                    composite.Add(new Leaf<bool>(fieldId) { Value = (bool) value });
                }
                else if ( type == typeof(int))
                {
                    composite.Add(new Leaf<int>(fieldId) { Value = (int)value });
                }
                else if (type == typeof(string))
                {
                    composite.Add(new Leaf<string>(fieldId) { Value = (string) value });
                }
                else if ( type == typeof(decimal) )
                {
                    composite.Add(new Leaf<decimal>(fieldId) { Value = (decimal) value });
                }
                else if (type == typeof(DateTime))
                {
                    composite.Add(new Leaf<DateTime>(fieldId) {Value = (DateTime) value});
                }
                else
                {
                    // Not known type, error
                    throw new ArgumentException($"Value type, {type}, is not known."); 
                }
            }
        }
    }
}
