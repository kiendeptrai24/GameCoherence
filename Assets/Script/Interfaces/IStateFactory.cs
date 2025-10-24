using System;
using System.Collections.Generic;
using UnityEngine;
public interface IStateFactory
{
    Dictionary<Type,IState> CreateState();
}