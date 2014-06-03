if(!isFunction(blastOff_NewPreload))
	exec("./blastoff.cs");
if(!isObject(psVoxBlockGroup))
	psVoxBlockGroup_New();

if(!isObject(psVoxBlockData_Dirt2x))
{
	new ScriptObject(psVoxBlockData_Dirt2x)
	{
		class = "psVoxBlockData_Trench";
		superClass = "psVoxBlockData";
		name = "Trench Dirt";

		shapeType = "Block";
		shape = blastOff_NewPreload("BDDirt", "STRING", "2x Cube\" 0 0 0 0 1" SPC getClosestPaintColor("0.53 0.3 0 1") SPC "0 0 0 1 1 1");
		solid = true;
		opaque = true;
		trigger = false;
		touch = false;
		posFromBottom = false;
		rotates = false;
		activated = false;
		carryProps = false;
		gravity = false;
		stable = true;

		breakSpeed = 5;
		weight = 1;
		support = -1;
	};
}

if(!isObject(psVoxBlockData_Empty))
{
	new ScriptObject(psVoxBlockData_Empty)
	{
		class = "psVoxBlockData_Trench";
		superClass = "psVoxBlockData";
		name = "Empty";

		shapeType = "Empty";
		solid = false;
		opaque = false;
		trigger = false;
		touch = false;
		posFromBottom = false;
		rotates = false;
		activated = false;
		carryProps = false;
		gravity = false;
		stable = true;

		breakSpeed = 5;
		weight = 1;
		support = -1;
	};
}