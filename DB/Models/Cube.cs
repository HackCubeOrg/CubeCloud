﻿using System;
using System.Collections.Generic;

namespace DB.Models
{
    public class Cube
    {
        public Guid Id { get; set; }

        public string Address { get; set; }

        public string Type { get; set; }

        public string Name { get; set; }

        public virtual void ProcessMessage(string messag)
        {
            throw new NotImplementedException();
        }
    }
}