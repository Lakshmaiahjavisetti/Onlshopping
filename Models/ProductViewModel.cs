﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mvc_systemtest.Models
{
    public class ProductViewModel
    {
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public decimal Price { get; set; }
        public string ImagePath { get; set; }
    }

}
