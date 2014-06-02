$PsVox::UpdateTick = 100;
$PsVox::UpdateReach = 1;

function psVoxBlockData::onBreak(%this, %obj, %player)
{
	//pass
}

// function psVoxBlockData::onUpdate(%this, %obj, %trigger)
// {
// 	//pass
// }

// function psVoxBlock::update(%this)
// {
// 	%this.type.schedule($PsVox::UpdateTick, onUpdate, %this, %this);
// 	%this.psVox.startBoxSearch(%this.pos, $PsVox::UpdateReach);
// 	while((%obj = %this.psVox.boxSearchNext()) != 0)
// 	{
// 		if(%obj == -1)
// 			continue;

// 		%obj.type.shedule(1$PsVox::UpdateTick, onUpdate, %obj, %this);
// 	}
// }

function psVox::startBoxSearch(%ps, %pos, %range, %this)
{
	if(!isObject(%this))
		%this = %ps;

	%x = getWord(%pos, 0);
	%y = getWord(%pos, 1);
	%z = getWord(%pos, 2);
	%minX = %x - %range;
	%minY = %y - %range;
	%minZ = %z - %range;
	%maxX = %x + %range;
	%maxY = %y + %range;
	%maxZ = %z + %range;

	%min = %minX SPC %minY SPC %minZ;
	%max = %maxX SPC %maxY SPC %maxZ;
	%box = %min SPC %max;
	if(%this.class $= "psVoxBlock")
	{
		%this.setProp("boxSearch", %pos SPC %range SPC %box);
		%this.setProp("boxCurr", %min);
		return;
	}
	%this.boxSearch = %pos SPC %range SPC %box;
	%this.boxCurr = %min;
}

function psVox::startBoxSearch2(%ps, %pos, %box, %this)
{
	if(!isObject(%this))
		%this = %ps;

	%x = getWord(%pos, 0);
	%y = getWord(%pos, 1);
	%z = getWord(%pos, 2);
	%minX = %x - getWord(%box, 0);
	%minY = %y - getWord(%box, 1);
	%minZ = %z - getWord(%box, 2);
	%maxX = %x + getWord(%box, 3);
	%maxY = %y + getWord(%box, 4);
	%maxZ = %z + getWord(%box, 5);

	%min = %minX SPC %minY SPC %minZ;
	%max = %maxX SPC %maxY SPC %maxZ;
	%box = %min SPC %max;
	%this.boxSearch = %pos SPC %range SPC %box;
	%this.boxCurr = %min;
}

function psVox::boxSearchNext(%this)
{
	if(%this.boxSearch $= "")
		return 0;

	%cX = getWord(%this.boxCurr, 0)+1;
	%cY = getWord(%this.boxCurr, 1);
	%cZ = getWord(%this.boxCurr, 2);

	%box = getWords(%this.boxSearch, 4, 9);
	%minX = getWord(%box, 0);
	%minY = getWord(%box, 1);
	%minZ = getWord(%box, 2);
	%maxX = getWord(%box, 3);
	%maxY = getWord(%box, 4);
	%maxZ = getWord(%box, 5);

	if(%cX > %maxX)
	{
		%cX = %minX;
		%cY++;
	}
	if(%cY > %maxY)
	{
		%cY = %minY;
		%cZ++;
	}
	if(%cZ > %maxZ)
	{
		%this.boxSearch = "";
		%this.boxCurr = "";
		return 0;
	}
	%this.boxCurr = %cX SPC %cY SPC %cZ;
	return %this.getBlock(%cX, %cY, %cZ);
}

function psVoxBlock::boxSearchNext(%this)
{
	%this.boxSearch = %this.getProp("boxSearch");
	%this.boxCurr = %this.getProp("boxCurr");
	if(%this.boxSearch $= "")
		return 0;

	%cX = getWord(%this.boxCurr, 0)+1;
	%cY = getWord(%this.boxCurr, 1);
	%cZ = getWord(%this.boxCurr, 2);

	%box = getWords(%this.boxSearch, 4, 9);
	%minX = getWord(%box, 0);
	%minY = getWord(%box, 1);
	%minZ = getWord(%box, 2);
	%maxX = getWord(%box, 3);
	%maxY = getWord(%box, 4);
	%maxZ = getWord(%box, 5);

	if(%cX > %maxX)
	{
		%cX = %minX;
		%cY++;
	}
	if(%cY > %maxY)
	{
		%cY = %minY;
		%cZ++;
	}
	if(%cZ > %maxZ)
	{
		%this.setProp("boxSearch", "");
		%this.setProp("boxCurr", "");
		return 0;
	}
	%this.setProp("boxCurr", %cX SPC %cY SPC %cZ);
	return %this.psVox.getBlock(%cX, %cY, %cZ);
}

// function psVoxSearchSlave::boxSearchNext(%this)
// {
// 	if(%this.boxSearch $= "")
// 		return 0;

// 	%cX = getWord(%this.boxCurr, 0)+1;
// 	%cY = getWord(%this.boxCurr, 1);
// 	%cZ = getWord(%this.boxCurr, 2);

// 	%box = getWords(%this.boxSearch, 4, 9);
// 	%minX = getWord(%box, 0);
// 	%minY = getWord(%box, 1);
// 	%minZ = getWord(%box, 2);
// 	%maxX = getWord(%box, 3);
// 	%maxY = getWord(%box, 4);
// 	%maxZ = getWord(%box, 5);

// 	if(%cX > %maxX)
// 	{
// 		%cX = %minX;
// 		%cY++;
// 	}
// 	if(%cY > %maxY)
// 	{
// 		%cY = %minY;
// 		%cZ++;
// 	}
// 	if(%cZ > %maxZ)
// 	{
// 		%this.boxSearch = "";
// 		%this.boxCurr = "";
// 		return 0;
// 	}
// 	%this.boxCurr = %cX SPC %cY SPC %cZ;
// 	return %this.psVox.getBlock(%cX, %cY, %cZ);
// }

// function psVoxBlock::newSearchSlave(%this, %pos, %range)
// {
// 	%slaves = %this.getProp("searchSlaves");

// 	%obj = new ScriptObject()
// 			{
// 				class = "psVoxSearchSlave";
// 				psVox = %this.psVox;
// 				parent = %this;
// 			};
// 	%this.psVox.startBoxSearch(%pos, %range, %obj);
// 	%this.setProp("searchSlave" @ %slaves, %obj);
// 	%this.setProp("searchSlaves", %slaves++);
// 	return %obj;
// }

// function psVoxBlock::searchSlaveNext(%this, %i)
// {
// 	%slaves = %this.getProp("searchSlaves");
// 	if(!%slaves || %i >= %slaves)
// 		return 0;

// 	%obj = %this.getProp("searchSlave" @ %i);
// 	%r = %obj.boxSearchNext();
	
// }