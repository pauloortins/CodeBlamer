﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBlamer.Infra.Models
{
    public class Metrics
    {
        public int MaintainabilityIndex { get; set; }
        public int CyclomaticComplexity { get; set; }
        public int ClassCoupling { get; set; }
        public int DepthOfInheritance { get; set; }
        public int LinesOfCode { get; set; }
    }
}