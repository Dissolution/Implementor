﻿using Implementor;

namespace ConsoleTester.Interfaces;

[Implement]
// [Entity]
public interface ITest
{
    int Id { get; set; }
    string? Name { get; set; }
}