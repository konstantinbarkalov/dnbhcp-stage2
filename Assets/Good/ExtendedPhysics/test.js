function ElipsoidInertiaTensorToCrossSize(inertiaTensor, mass) {
    const sumOfSquaredHalfsizes = inertiaTensor;
    const x = Math.sqrt((sumOfSquaredHalfsizes.y + sumOfSquaredHalfsizes.z - sumOfSquaredHalfsizes.x) / mass * 5 / 2) * 2;
    const y = Math.sqrt((sumOfSquaredHalfsizes.x + sumOfSquaredHalfsizes.z - sumOfSquaredHalfsizes.y) / mass * 5 / 2) * 2;
    const z = Math.sqrt((sumOfSquaredHalfsizes.x + sumOfSquaredHalfsizes.y - sumOfSquaredHalfsizes.z) / mass * 5 / 2) * 2;
    const crossSize = { x, y, z };
    return crossSize;
}
function CrossSizeToElipsoidInertiaTensor(crossSize, mass) {
    const x = (crossSize.y * crossSize.y + crossSize.z * crossSize.z) / 4 * mass / 5;
    const y = (crossSize.x * crossSize.x + crossSize.z * crossSize.z) / 4 * mass / 5;
    const z = (crossSize.x * crossSize.x + crossSize.y * crossSize.y) / 4 * mass / 5;
    const sumOfSquaredHalfsizes = { x, y, z };
    const inertiaTensor = sumOfSquaredHalfsizes;
    return inertiaTensor;
}
var mass = 1;
var crossSize = {x: 1, y: 2, z: 3};
var inertiaTensor = {x: 1/5 * mass * (3 * 3 + 4 * 4), y: 1/5 * mass * (3 * 3 + 5 * 5), z: 1/5 * mass * (4 * 4 + 5 * 5)};


function testForeward(crossSize, mass) {
    var inertiaTensor = CrossSizeToElipsoidInertiaTensor(crossSize, mass);
    var testCrossSize = ElipsoidInertiaTensorToCrossSize(inertiaTensor, mass);
    console.log("testForeward");
    console.log("crossSize", crossSize);
    console.log("inertiaTensor", inertiaTensor);
    console.log("testCrossSize", testCrossSize);
}

function testBackward(inertiaTensor, mass) {
    var crossSize = ElipsoidInertiaTensorToCrossSize(inertiaTensor, mass);
    var testInertiaTensor = CrossSizeToElipsoidInertiaTensor(crossSize, mass);
    console.log("testBackward");
    console.log("inertiaTensor", inertiaTensor);
    console.log("crossSize", crossSize);
    console.log("testInertiaTensor", testInertiaTensor);
}
function test() {
    testForeward(crossSize, mass);
    testBackward(inertiaTensor, mass);
}
test();