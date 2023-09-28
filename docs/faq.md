# FAQ

## How to remove namespace from embedded scripts?

We do not recommend this. It has some pitfalls. See the linked source. But if you really want to. Add to the csproj file:

```
 <ItemGroup>
    <EmbeddedResource Include="Database\Migrations\*.sql">
      <LogicalName>%(filename)%(extension)</LogicalName>
    </EmbeddedResource>
  </ItemGroup>
```

Source: [https://github.com/DbUp/DbUp/issues/166](https://github.com/DbUp/DbUp/issues/166)
