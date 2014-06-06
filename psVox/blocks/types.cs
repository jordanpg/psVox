if(!isFunction(blastOff_NewPreload))
	exec("./blastoff.cs");
if(!isObject(psVoxBlockGroup))
	psVoxBlockGroup_New();

if(!isObject(psVoxBlockData_Wall))
{
	new ScriptObject(psVoxBlockData_Wall)
	{
		class = "psVoxBlockData";
		name = "Wall";

		shapeType = "Block";
		shape = blastOff_NewPreload("BDWall", "STRING", "4x Cube\" 0 0 0 0 1" SPC getClosestPaintColor("0.5 0.5 0.5 1") SPC "0 0 0 1 1 1");
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

		breakSpeed = 15;
		weight = 10;
		support = -1;
	};
}

if(!isObject(psVoxBlockData_Glass))
{
	new ScriptObject(psVoxBlockData_Glass)
	{
		class = "psVoxBlockData";
		name = "Glass";

		shapeType = "Block";
		shape = blastOff_NewPreload("BDGlass", "STRING", "4x Cube\" 0 0 0 0 1" SPC getClosestPaintColor("1 1 1 0.2") SPC "0 0 0 1 1 1");
		solid = true;
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

if(!isObject(psVoxBlockData_Test))
{
	new ScriptObject(psVoxBlockData_Test)
	{
		class = "psVoxBlockData";
		name = "Test";

		shapeType = "Save";
		shape = blastOff_NewPreload("BDTest", "BLS", "saves/shape_testCell.bls", 1);
		solid = false;
		opaque = false;
		trigger = false;
		touch = false;
		posFromBottom = true;
		rotates = true;
		activated = true;
		carryProps = false;
		gravity = false;
		stable = true;

		breakSpeed = 50;
		weight = 25;
		support = -1;
	};
}

if(!isObject(psVoxBlockData_SupportT1))
{
	new ScriptObject(psVoxBlockData_SupportT1)
	{
		class = "psVoxBlockData";
		name = "Wood Support";

		shapeType = "Save";
		shape = blastOff_NewPreload("BDS1", "BLS", "saves/shape_support1.bls", 1);
		solid = false;
		opaque = false;
		trigger = false;
		touch = false;
		posFromBottom = true;
		rotates = false;
		activated = false;
		carryProps = false;
		gravity = false;
		stable = true;

		breakSpeed = 75;
		weight = 5;
		support = 5;

		supportTier = 1;
	};

	$psVox::SupportTier1 = psVoxBlockData_SupportT1;
}

if(!isObject(psVoxBlockData_SupportT2))
{
	new ScriptObject(psVoxBlockData_SupportT2 : psVoxBlockData_SupportT1)
	{
		name = "Reinforced Wood Support";

		shape = blastOff_NewPreload("BDS2", "BLS", "saves/shape_support2.bls", 1);

		support = 10;

		supportTier = 2;
	};

	$psVox::SupportTier2 = psVoxBlockData_SupportT2;
}

if(!isObject(psVoxBlockData_SupportT3))
{
	new ScriptObject(psVoxBlockData_SupportT3 : psVoxBlockData_SupportT1)
	{
		name = "Stone Support";

		shape = blastOff_NewPreload("BDS3", "BLS", "saves/shape_support3.bls", 1);

		weight = 10;
		support = 25;

		supportTier = 3;
	};

	$psVox::SupportTier3 = psVoxBlockData_SupportT3;
}

if(!isObject(psVoxBlockData_SupportT4))
{
	new ScriptObject(psVoxBlockData_SupportT4 : psVoxBlockData_SupportT1)
	{
		name = "Metal Support";

		shape = blastOff_NewPreload("BDS4", "BLS", "saves/shape_support4.bls", 1);

		weight = 20;
		support = 40;

		supportTier = 4;
	};

	$psVox::SupportTier4 = psVoxBlockData_SupportT4;
}

if(!isObject(psVoxBlockData_SupportT5))
{
	new ScriptObject(psVoxBlockData_SupportT5 : psVoxBlockData_SupportT1)
	{
		name = "Strong Metal Support";

		shape = blastOff_NewPreload("BDS5", "BLS", "saves/shape_support5.bls", 1);

		weight = 30;
		support = 75;

		supportTier = 5;
	};

	$psVox::SupportTier5 = psVoxBlockData_SupportT5;
}

function psVoxBlockData_Test::onActivated(%this, %block, %obj, %ray)
{
	%pos = getWords(%ray, 1, 3);
	ServerPlay3D(BrickPlantSound, %pos);
	if(isObject(%obj.client))
		messageClient(%obj.client, '', "\c6boom bap");
}