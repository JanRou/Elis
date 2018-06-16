﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockImportService.Pipeline {
    public interface IOperation<T> {
        IEnumerable<T> Execute(IEnumerable<T> input);
    }
}
