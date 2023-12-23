﻿using Services.Common.Domain.Exceptions;
using Services.Common.Domain;
using Services.Common.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Common.Domain.ValueObjects
{
    public class PhoneNumber : BaseValueObject
    {
        private PhoneNumber()
        {

        }
        public PhoneNumber(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || value.IsText() || value.Length is < 11 or > 11)
                throw new InvalidDomainDataException("شماره تلفن نامعتبر است");
            Value = value;
        }

        public string Value { get; private set; }
    }
}