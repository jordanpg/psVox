$PsVox::TrenchHeight = 4;
$PsVox::TrenchDlgSpeed = 500;
$PsVox::TrenchPlantSpeed = 300;
$PsVox::TrenchBreakEfficiency = 1;
$PsVox::TrenchPlaceRequirement = 1;
$PsVox::TrenchPlaceType = psVoxBlockData_Dirt2x;
$PsVox::TrenchDirtUnit = "cm^3";
$PsVox::TrenchMaxDirt = 500;

function genTrenchMap(%gridSize, %dist, %bperc, %brand)
{
	if(%bperc > 1)
		%bperc /= 100;
	%gX = getWord(%gridSize, 0);
	%gY = getWord(%gridSize, 1);

	%hyp = mFloor(mSqrt(((%gX) * (%gX) + (%gY) * (%gY)) + 0.5));
	if(%dist > 0)
	{
		if(%dist > %hyp)
		{
			echo("Distance does not fit in map!");
			return 0;
		}

		%pos1 = getRandom(0, %gX) SPC getRandom(0, %gX);
		%pos2 = "";
		for(%y = 0; %y <= %gY; %y++)
		{
			for(%x = 0; %x < %gX; %x++)
			{
				%pos = %x SPC %y;
				%d = mFloor(VectorDist(%pos1, %pos2) + 0.5);

				if(%d >= %dist)
				{
					%pos2 = %pos;
					break;
				}
			}
			if(%pos2 !$= "")
					break;
		}
		if(%pos2 $= "")
		{
			echo("Could not find a suitable location for a second base!");
			return 0;
		}
	}
	else
	{
		%pos1 = "0 0";
		%pos2 = %gridSize;
	}

	%map = new ScriptObject(TrenchMap)
			{
				gX = %gX;
				gY = %gY;
			};
	for(%y = 0; %y <= %gY; %y++)
	{
		for(%x = 0; %x <= %gX; %x++)
		{
			%map.map[%x, %y] = 0;
		}
	}

	%x1 = getWord(%pos1, 0);
	%y1 = getWord(%pos1, 1);
	%x2 = getWord(%pos2, 0);
	%y2 = getWord(%pos2, 1);
	if(%x2 < %x1)
	{
		%t = %x1;
		%x1 = %x2;
		%x2 = %t;
	}
	if(%y2 < %y1)
	{
		%t = %y1;
		%y1 = %y2;
		%y2 = %t;
	}
	%map.map[%x1, %y1] = -1;
	%map.map[%x2, %y2] = -2;

	%midX = mFloor(((%x1 + %x2) / 2) + 0.5);
	%midY = mFloor(((%y1 + %y2) / 2) + 0.5);

	%perc = mFloor(%gX * %bperc);
	if(%brand)
		%fieldSize = getRandom(0, %perc);
	else
		%fieldSize = %perc;
	%half = mFloor(%fieldSize / 2);

	%minX = %midX - %half;
	if(%minX < %x1)
		%minX = %x1;
	else
		%pX1 = true;
	%minY = %midY - %half;
	if(%minY < %y1)
		%minY = %y1;
	else
		%pY1 = true;
	%maxX = %midX + %half;
	if(%maxX > %x2)
		%maxX = %x2;
	else
		%pX2 = true;
	%maxY = %midY + %half;
	if(%maxY > %y2)
		%maxY = %y2;
	else
		%pY2 = true;

	echo(%minX SPC %minY SPC %maxX SPC %maxY);
	for(%y = %minY; %y <= %maxY; %y++)
	{
		for(%x = %minX; %x <= %maxX; %x++)
		{
			%cell = %map.map[%x, %y];
			if(%cell < 0)
			{
				%val = mAbs(%cell);
				%pX[%val] = false;
				%pY[%val] = false;
				continue;
			}
			%map.map[%x, %y] = 2;
		}
	}

	if(%pX1 || %pY1)
	{
		%path = false;
		%cX = %x1;
		%cY = %y1;
		while(%pX1)
		{
			%cX++;
			if(%cX >= %minX)
			{
				echo("end x1" SPC %cX SPC %cY);
				%curr = %map.map[%cX, %cY];
				if(%curr == 0)
					%map.map[%cX, %cY] = 1;
				%pX1 = false;
			}
			else
			{
				%curr = %map.map[%cX, %cY];
				if(%curr == 2)
				{
					echo("make x1" SPC %cX SPC %cY);
					%path = true;
					break;
				}
				else
				{
					echo("plant x1" SPC %cX SPC %cY);
					%map.map[%cX, %cY] = 1;
				}
			}
		}

		if(%path)
		{
			%pX1 = false;
			%pY1 = false;
		}

		while(%pY1)
		{
			%cY++;
			if(%cY >= %minY)
			{
				echo("end Y1" SPC %cY SPC %cY);
				%curr = %map.map[%cX, %cY];
				if(%curr == 0)
					%map.map[%cX, %cY] = 1;
				%pY1 = false;
			}
			else
			{
				%curr = %map.map[%cX, %cY];
				if(%curr == 2)
				{
					echo("make Y1" SPC %cY SPC %cY);
					%path = true;
					break;
				}
				else
				{
					echo("plant Y1" SPC %cY SPC %cY);
					%map.map[%cX, %cY] = 1;
				}
			}
		}
		if(%cX >= %minX && %cY >= %minY)
			%path = true;
		if(!%path)
		{
			echo("Couldn't make path from pos1 to the battlefield.");
			return %map;
		}
	}

	if(%pX2 || %py2)
	{
		%path = false;
		%cX = %X2;
		%cY = %y2;
		while(%pX2)
		{
			%cX--;
			if(%cX <= %maxX)
			{
				echo("end X2" SPC %cX SPC %cY);
				%curr = %map.map[%cX, %cY];
				if(%curr == 0)
					%map.map[%cX, %cY] = 1;
				%pX2 = false;
			}
			else
			{
				%curr = %map.map[%cX, %cY];
				if(%curr == 2)
				{
					echo("make X2" SPC %cX SPC %cY);
					%path = true;
					break;
				}
				else
				{
					echo("plant X2" SPC %cX SPC %cY);
					%map.map[%cX, %cY] = 1;
				}
			}
		}

		if(%path)
		{
			%pX2 = false;
			%pY2 = false;
		}

		while(%py2)
		{
			%cY--;
			if(%cY <= %maxY)
			{
				echo("end y2" SPC %cY SPC %cY);
				%curr = %map.map[%cX, %cY];
				if(%curr == 0)
					%map.map[%cX, %cY] = 1;
				%py2 = false;
			}
			else
			{
				%curr = %map.map[%cX, %cY];
				if(%curr == 2)
				{
					echo("make y2" SPC %cY SPC %cY);
					%path = true;
					break;
				}
				else
				{
					echo("plant y2" SPC %cY SPC %cY);
					%map.map[%cX, %cY] = 1;
				}
			}
		}
		if(%cX <= %maxX && %cY <= %maxY)
			%path = true;
		if(!%path)
		{
			echo("Couldn't make path from pos2 to the battlefield.");
			return %map;
		}
	}
	%map.done = true;
	return %map;
}

function trenchMap::debug(%this, %origin)
{
	%c[-1] = 0;
	%c[-2] = 3;
	%c[0] = 17;
	%c[1] = 8;
	%c[2] = 9;

	%i = 0;
	for(%y = 0; %y <= %this.gY; %y++)
	{
		for(%x = 0; %x <= %this.gX; %x++)
		{
			%cell = %this.map[%x, %y];

			%col = %c[%cell];
			if(%col $= "")
				%col = 5;
			%pos = %x SPC %y SPC 0;
			%pos = VectorAdd(%pos, %origin);
			schedule(%i * 33, 0, simpBrick, Brick2x2Data, %pos, localClientConnection, %col);
			%i++;
		}
	}
}

function psVoxBlockData_Trench::onBreak(%this, %obj, %player)
{
	%d0 = %obj.getRelative("0 0 1");
	%d1 = %obj.getRelative("0 0 -1");
	%d2 = %obj.getRelative("-1 0 0");
	%d3 = %obj.getRelative("1 0 0");
	%d4 = %obj.getRelative("0 -1 0");
	%d5 = %obj.getRelative("0 1 0");
	for(%i = 0; %i < 6; %i++)
	{
		%o = %d[%i];
		if(!isObject(%o))
			continue;

		if(%o.type.getID() == psVoxBlockData_Empty.getID())
		{
			// echo(%o.type.name);
			// echo(%o.type);
			// echo(psVoxBlockData_Empty);
			%o.setType(psVoxBlockData_Dirt2x, 1);
		}
	}

	%cl = %player.client;
	if(isObject(%cl) && %cl.trenchBuild && %cl.trenchDirt < $PsVox::TrenchMaxDirt)
	{
		%add = $PsVox::TrenchBreakEfficiency * %this.trenchEfficiency;
		%cl.trenchDirt += %add;
	}
	if(%cl.trenchDirt > $PsVox::TrenchMaxDirt)
		%cl.trenchDirt = $PsVox::TrenchMaxDirt;

	parent::onBreak(%this, %obj, %player);
}

function psVoxBlockData_Trench::onBlockSet(%this, %obj, %overrideKeep, %noupdate)
{
	%below = %obj.getRelative("0 0 -1");
	if(isObject(%below) && %below.type.getID() == psVoxBlockData_Grass2x.getID())
		%below.setType(psVoxBlockData_Dirt2x, 1);
	parent::onBlockSet(%this, %obj, %overrideKeep, %noupdate);
}

// function psVoxGenQueue::onQueueFinished(%this)
// {
// 	switch($psVoxTrench_Phase)
// 	{
// 		case 1: %this.addJobToBack(psVoxGen_Trench_1, %this.psVox, %size, %map, %scales, %grass, 0, 0, 0);
// 	}
// }

function psVoxGen_Trench(%this, %size, %bperc, %brand, %scales, %grass)
{
	%map = genTrenchMap(%size, 0, %bperc, %brand);
	if(!%map.done)
	{
		%map.delete();
		return;
	}
	// %minX = 0;
	// %minY = 0;
	// %maxX = getWord(%size, 0);
	// %maxY = getWord(%size, 1);
	// %this.Gen_Chunks(%minX, %minY, 0, %maxX, %maxY, $PsVox::TrenchHeight);
	$psVoxTrench_Phase = 1;

	%this.genQueue.addJobToBack(psVoxGen_Trench_1, %this.psVox, %size, %map, %scales, %grass, 0, 0, 0);
}

function psVoxGen_Trench_1(%this, %size, %map, %scales, %grass, %cX, %cY, %cZ)
{
	%minX = 0;
	%minY = 0;
	%maxX = getWord(%size, 0);
	%maxY = getWord(%size, 1);

	%c = %map.map[%cX, %cY];

	if(%c != 0)
		%this.Gen_Simplex(%cX, %cY, %cZ, %scales, %grass);

	%cX++;
	if(%cX > %maxX)
	{
		%cX = 0;
		%cY++;
	}
	if(%cY > %maxY)
	{
		%cY = 0;
		%cZ++;
	}
	if(%cZ <= $PsVox::TrenchHeight)
	{
		%this.genQueue.addJobToBack(psVoxGen_Trench_1, %this, %size, %map, %scales, %grass, %cX, %cY, %cZ);
	}
	else
		$psVoxTrench_Phase = 2;
}

// function psVoxGen_Trench_2(%this)

function PsVox::Gen_Trench(%this, %size, %bperc, %brand, %scales, %grass, %addheight, %seed, %freq, %iter, %persist, %low, %high)
{
	if(!isObject(%this.genQueue))
		%this.initGen();

	if(!isObject(%this.terrain))
		%this.initSimplex(%seed, %freq, %iter, %persist, %low, %high, %addheight);

	$psVoxGen_Phase = 0;
	%this.genQueue.addJobToBack(psVoxGen_Trench, %this, %size, %bperc, %brand, %scales, %grass);
}

function trenchDlg(%this, %player)
{
	if(%this.trenchBuild)
		%this.trenchDlg();
}

function GameConnection::trenchDlg(%this)
{
	if(isEventPending(%this.trenchDlg))
		cancel(%this.trenchDlg);

	if(!%this.trenchBuild)
		return;

	%dirt = %this.trenchDirt;
	%blocks = %dirt / $PsVox::TrenchPlaceRequirement;

	%msg = "\c3DIRT\c6:" SPC mFloatLength(%blocks, 2) @ $PsVox::TrenchDirtUnit;
	if(isObject(%this.player) && %this.player.breakProg > 0)
	{
		for(%i = 0; %i < 50; %i++)
			%str = %str @ (%i <= %this.player.breakProg ? "\c3" : "\c0") @ "|";
		%msg = %msg TAB %str;
	}
	%this.bottomPrint(strReplace(%msg, "\t", "\n"), 1, 1);

	%this.trenchDlg = %this.schedule($PsVox::TrenchDlgSpeed, trenchDlg);
}

function Player::trenchPlant(%this)
{
	if(isEventPending(%this.trenchPlant))
		cancel(%this.trenchPlant);

	%cl = %this.client;
	if(!isObject(%cl))
		return;

	%bl = %this.getBlockAim();
	if(isObject(%bl) && %bl.type.getID() == nameToID(psVoxBlockData_None))
	{
		%dirt = %cl.trenchDirt;
		if(%dirt >= $PsVox::TrenchPlaceRequirement)
		{
			%this.playThread(3, activate);
			%bln = %bl.setType($PsVox::TrenchPlaceType);
			// echo(%bln SPC %bln.type.name);
			if(isObject(%bln))
			{
				ServerPlay3D(BrickPlantSound, PsVox.getRealPos(%bl.pos));
				%cl.trenchDirt -= $PsVox::TrenchPlaceRequirement;
			}
		}
	}

	%this.trenchPlant = %this.schedule($PsVox::TrenchPlantSpeed, trenchPlant);
}

package psVoxTrench
{
	function Armor::onTrigger(%this, %obj, %slot, %val)
	{
		%r = parent::onTrigger(%this, %obj, %slot, %val);

		if(!isObject(%c = %obj.client))
			return %r;
		if(!%c.trenchBuild || %c.voxBuild)
			return %r;

		%look = %obj.getBlockLook();
		%lbl = getField(%look, 0);
		if(%slot $= 4)
		{
			if(%val)
			{
				if(isEventPending(%obj.blockBreak))
				{
					cancel(%obj.blockBreak);
					%obj.breakProg = 0;
					if(isObject(%lbl))
						%lbl.setRotation(%lbl.rotation + (%obj.isCrouched() ? 1 : -1));
					return %r;
				}

				if(isObject(%lbl) && (%lbl.type.activated && !%obj.isCrouched()))
				{
					%ray = getField(%look, 1);
					%lbl.type.onActivated(%lbl, %obj, %ray);
					return %r;
				}

				%obj.trenchPlant();
			}
			else
			{
				if(isEventPending(%obj.trenchPlant))
					cancel(%obj.trenchPlant);
				%obj.playThread(3, root);
			}
		}
		else if(%slot $= 0)
		{
			if(%val)
			{
				if(isObject(%lbl))
				{
					%obj.client.voxBreak = true;
					%obj.client.voxNoProg = true;
					%obj.client.voxProgF = trenchDlg;
					%obj.breakProg = 0;
					%obj.blockBreak(%lbl);
					%obj.playThread(3, activate2);
				}
			}
			else
			{
				if(isEventPending(%obj.blockBreak))
					cancel(%obj.blockBreak);
				%obj.breakProg = 0;
				%obj.playThread(3, root);
				%obj.client.voxBreak = false;
				%obj.client.voxNoProg = false;
				%obj.client.voxProgF = "";
			}
		}
		return %r;
	}
};
activatePackage(psVoxTrench);