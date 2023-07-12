﻿using System;

namespace Tharga.Toolkit.TypeService;

public class TypeMissingException : Exception
{
    public TypeMissingException(Exception innerException, string message)
        : base(message, innerException)
    {
    }

    public TypeMissingException(string typeName)
        : base($"Unable to find type {typeName}.")
    {
    }
}