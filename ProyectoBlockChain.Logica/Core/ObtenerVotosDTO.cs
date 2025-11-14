using Nethereum.ABI.FunctionEncoding.Attributes;
using ProyectoBlockChain.Logica.Core;
using System.Collections.Generic;

[FunctionOutput]
public class ObtenerVotosOutputDTO : IFunctionOutputDTO
{
    [Parameter("tuple[]", "", 1)]
    public List<VotoDTO> Votos { get; set; }
}