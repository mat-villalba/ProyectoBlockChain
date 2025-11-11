import hre from "hardhat";

async function main() {
    const [deployer] = await hre.ethers.getSigners();
    console.log("Desplegando contrato con la cuenta:", deployer.address);

    // ✅ Forma correcta de obtener balance en ethers v6
    const balance = await hre.ethers.provider.getBalance(deployer.address);
    console.log("Balance de deployer:", balance.toString());

    const AventuraChain = await hre.ethers.getContractFactory("AventuraChainV2");
    const aventura = await AventuraChain.deploy();
    await aventura.waitForDeployment();

    const address = await aventura.getAddress();
    console.log("✅ Contrato desplegado en:", address);
}

main().catch((error) => {
    console.error(error);
    process.exitCode = 1;
});
