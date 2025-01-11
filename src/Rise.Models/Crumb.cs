﻿using System;

namespace Rise.Models
{
    public struct Crumb
    {
        public string Title { get; set; }
        public Type Type { get; private set; }

        public Crumb(string title, Type type)
        {
            Title = title;
            Type = type;
        }

        public override string ToString()
        {
            return Title;
        }
    }
}
