﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Tables {

    public interface IId {
        int Id { get; set; } // set for unit testing
    }
}
