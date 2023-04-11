### To Implement
- Some way to generate:
  - ```
    foreach (ctor)
    {   
        `T ..ctor(cParams)`
        emit:
        T? New(cParams?)
    }    
```