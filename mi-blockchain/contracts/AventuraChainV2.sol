// SPDX-License-Identifier: MIT 

pragma solidity ^0.8.19; 

 

 

/** 

 * @title AventuraChainV2 

 * @notice Registra votaciones individuales y resultados de partidas de un juego de decisiones. 

 */ 

contract AventuraChainV2 { 

 

 

    // --- 1️⃣ ESTRUCTURAS --- 

     

    struct Voto { 

        address jugador;      // dirección de la wallet 

        uint capituloId; 

        string opcionElegida; // "Opcion1" o "Opcion2" 

        uint timestamp; 

    } 

 

 

    struct DecisionFinal { 

        uint capituloId; 

        string opcionGanadora; 

        uint votosOpcionA; 

        uint votosOpcionB; 

        bool desempateAplicado; 

        uint timestamp; 

    } 

 

 

    // --- 2️⃣ ALMACENAMIENTO --- 

     

    // Mapea el ID de partida => lista de votos emitidos 

    mapping(uint => Voto[]) public votosPorPartida; 

     

    // Mapea el ID de partida => lista de decisiones finales 

    mapping(uint => DecisionFinal[]) public historialPartidas; 

     

    // Control de partidas 

    uint public proximaPartidaId = 1; 

    address public owner; 

 

 

    // --- 3️⃣ EVENTOS --- 

 

 

    event VotoRegistrado(uint indexed partidaId, address indexed jugador, uint capituloId, string opcion); 

    event DecisionFinalRegistrada(uint indexed partidaId, uint capituloId, string opcionGanadora); 

 

 

    // --- 4️⃣ MODIFICADOR --- 

 

 

    modifier onlyOwner() { 

        require(msg.sender == owner, "Solo el owner puede ejecutar esto"); 

        _; 

    } 

 

 

    constructor() { 

        owner = msg.sender; 

    } 

 

 

    // --- 5️⃣ FUNCIONES PRINCIPALES --- 

 

 

    /// Inicia una nueva partida y devuelve el ID 

    function iniciarNuevaPartida() public onlyOwner returns (uint) { 

        uint nuevaPartidaId = proximaPartidaId; 

        proximaPartidaId++; 

        return nuevaPartidaId; 

    } 

 

 

    /// Los jugadores registran su voto (firma + gas) 

    function votar(uint _partidaId, uint _capituloId, string memory _opcionElegida) public { 

        require(bytes(_opcionElegida).length > 0, "Debe indicar una opcion"); 

        require(_partidaId > 0 && _partidaId < proximaPartidaId, "Partida no valida"); 

 

 

        Voto memory nuevoVoto = Voto({ 

            jugador: msg.sender, 

            capituloId: _capituloId, 

            opcionElegida: _opcionElegida, 

            timestamp: block.timestamp 

        }); 

 

 

        votosPorPartida[_partidaId].push(nuevoVoto); 

        emit VotoRegistrado(_partidaId, msg.sender, _capituloId, _opcionElegida); 

    } 

 

 

    /// El backend registra el resultado final de una ronda 

    function registrarDecisionFinal( 

        uint _partidaId,  

        uint _capituloId,  

        string memory _opcionGanadora,  

        uint _votosA,  

        uint _votosB,  

        bool _desempate 

    ) public onlyOwner { 

 

 

        DecisionFinal memory nuevaDecision = DecisionFinal({ 

            capituloId: _capituloId, 

            opcionGanadora: _opcionGanadora, 

            votosOpcionA: _votosA, 

            votosOpcionB: _votosB, 

            desempateAplicado: _desempate, 

            timestamp: block.timestamp 

        }); 

 

 

        historialPartidas[_partidaId].push(nuevaDecision); 

        emit DecisionFinalRegistrada(_partidaId, _capituloId, _opcionGanadora); 

    } 

 

 

    // --- 6️⃣ FUNCIONES DE LECTURA --- 

     

    function obtenerVotos(uint _partidaId) public view returns (Voto[] memory) { 

        return votosPorPartida[_partidaId]; 

    } 

 

 

    function obtenerHistorialPartida(uint _partidaId) public view returns (DecisionFinal[] memory) { 

        return historialPartidas[_partidaId]; 

    } 

} 

 