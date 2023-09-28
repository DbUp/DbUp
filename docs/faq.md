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

_We recommend using it with namespaces only. DbUp is a community project with limited support. So we will not support you, if you run into problems._

Source: [https://github.com/DbUp/DbUp/issues/166](https://github.com/DbUp/DbUp/issues/166)
