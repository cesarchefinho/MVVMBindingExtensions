using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace System.ComponentModel.DataAnnotations;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
public class UpperCaseAttribute : Attribute
{
    public UpperCaseAttribute() : base()
    {
    }
}

