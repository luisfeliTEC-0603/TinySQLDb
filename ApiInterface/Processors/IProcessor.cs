﻿using ApiInterface.InternalModels;
using ApiInterface.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiInterface.Processors
{
    internal interface IProcessor
    {
        // Method to process a request and return a Response object
        Response Process();
    }
}