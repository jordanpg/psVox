if(!isFunction(blastOff_NewPreload))
	exec("./blastoff.cs");
if(!isObject(psVoxBlockGroup))
	psVoxBlockGroup_New();

if(!isObject(psVoxBlockData_Dirt))
{
	new ScriptObject(psVoxBlockData_Dirt)
	{
		class = "psVoxBlockData_Natural";
		superClass = "psVoxBlockData";
		name = "Dirt";

		shapeType = "Block";
		shape = blastOff_NewPreload("BDDirt", "STRING", "4x Cube\" 0 0 0 0 1" SPC getClosestPaintColor("0.53 0.3 0 1") SPC "0 0 0 1 1 1");
		shapeBasic = true;
		solid = true;
		opaque = true;
		trigger = false;
		touch = false;
		posFromBottom = false;
		rotates = false;
		activated = false;
		carryProps = false;
		gravity = false;
		stable = false;

		breakSpeed = 5;
		weight = 1;
		support = -1;
	};
}

if(!isObject(psVoxBlockData_Stone))
{
	new ScriptObject(psVoxBlockData_Stone : psVoxBlockData_Dirt)
	{
		name = "Stone";

		shape = blastOff_NewPreload("BDStone", "STRING", "4x Cube\" 0 0 0 0 1" SPC getClosestPaintColor("0.75 0.75 0.75 1") SPC "0 0 0 1 1 1");

		breakSpeed = 10;
	};
}

if(!isObject(psVoxBlockData_HardStone))
{
	new ScriptObject(psVoxBlockData_HardStone : psVoxBlockData_Dirt)
	{
		name = "Hard Stone";

		shape = blastOff_NewPreload("BDHardStone", "STRING", "4x Cube\" 0 0 0 0 1" SPC getClosestPaintColor("0.3 0.3 0.3 1") SPC "0 0 0 1 1 1");
		stable = true;

		breakSpeed = 25;
		weight = 5;
	};
}

if(!isObject(psVoxBlockData_Sand))
{
	new ScriptObject(psVoxBlockData_Sand : psVoxBlockData_Dirt)
	{
		name = "Sand";

		shape = blastOff_NewPreload("BDSand", "STRING", "4x Cube\" 0 0 0 0 1" SPC getClosestPaintColor("1 0.94 0.7 1") SPC "0 0 0 1 1 1");
		gravity = true;

		breakSpeed = 5;
	};
}