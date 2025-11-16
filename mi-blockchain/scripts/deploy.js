// scripts/deploy.js
const hre = require("hardhat");

async function main() {
  const [deployer] = await hre.ethers.getSigners();
  console.log("Deploying with account:", deployer.address);

  const Factory = await hre.ethers.getContractFactory("AventuraChainV2"); // pon tu contrato
  const contrato = await Factory.deploy();
  await contrato.waitForDeployment();

  console.log("Contrato desplegado en:", contrato.target);
}

main()
  .then(() => process.exit(0))
  .catch((error) => {
    console.error(error);
    process.exit(1);
  });