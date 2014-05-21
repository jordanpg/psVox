function psVox::buildRectRoom(%this, %origin, %l, %w, %h, %type, %delayFactor)
{
	if(%delayFactor <= 0)
		%delayFactor = 1;
	%l += 1;
	%w += 1;
	%h += 1;
	%sX = getWord(%origin, 0);
	%sY = getWord(%origin, 1);
	%sZ = getWord(%origin, 2);
	%fX = %x + %l;
	%fY = %y + %w;
	%fZ = %z + %h;
	%box = %sX SPC %sY SPC %sZ SPC %fX SPC %fY SPC %fZ;
	%i = 0;
	for(%z = %sZ; %z <= %fZ; %z++)
	{
		// if(%z > %sZ && %z < %fZ)
		// 	continue;
		for(%y = %sY; %y <= %fY; %y++)
		{
			// if(%y > %sY && %y < %fY)
			// 	continue;
			for(%x = %sX; %x <= %fX; %x++)
			{
				// if(%x > %sX && %x < %fX)
				// 	continue;
				if(pointInBox(%x SPC %y SPC %z, %box, 0))
					%this.schedule(%i * %delayFactor, setBlock, %x, %y, %z, psVoxBlockData_None);
				else
				{
					%this.schedule(%i * %delayFactor, setBlock, %x, %y, %z, %type);
					%i++;
				}
			}
		}
	}
}

function SimSet::SaveCell(%this, %file)
{
	%path = "saves/" @ %file @ ".bls";
	if(!isWriteableFileName(%path))
	{
		echo("a");
		return false;
	}
	%file = new FileObject();
	%file.openForWrite(%path);
	%file.writeLine("This is a Blockland save file.  You probably shouldn't modify it cause you'll screw it up.");
	%file.writeLine(1);
	%file.writeLine("");
	for(%i = 0; %i < 64; %i++)
		%file.writeLine(getColorIDTable(%i));

	%file.writeLine("Linecount " @ %this.getCount());
	%ct = 0;
	for(%i = 0; %i < %this.getCount(); %i++)
	{
		%brick = %this.getObject(%i);
		
		if(%brick.getDataBlock().hasPrint)
		{
			%texture = getPrintTexture(%brick.getPrintId());
			%path = filePath(%texture);
			%underscorePos = strPos(%path, "_");
			%name = getSubStr(%path, %underscorePos + 1, strPos(%path, "_", 14) - 14) @ "/" @ fileBase(%texture);
			if($printNameTable[%name] !$= "")
			{
				%print = %name;
			}
		}

		%file.writeLine(%brick.getDataBlock().uiName @ "\" " @ %this.position[%brick] SPC %brick.getAngleID() SPC %brick.isBasePlate() SPC %brick.getColorID() SPC %print SPC %brick.getColorFXID() SPC %brick.getShapeFXID() SPC %brick.isRayCasting() SPC %brick.isColliding() SPC %brick.isRendering());

		if(isObject(%brick.emitter))
			%file.writeLine("+-EMITTER " @ %brick.emitter.emitter.uiName @ "\" " @ %brick.emitterDirection);
		if(%brick.getLightID() >= 0)
			%file.writeLine("+-LIGHT " @ %brick.getLightID().getDataBlock().uiName @ "\" "); // Not sure if something else comes after the name
		%ct++;
	}
	%file.close();
	%file.delete();
	return %ct;
}

function fxDTSBrick::CreateCellGroup(%this)
{
	%group = new SimSet();
	%group.add(%this);
	%pos = "0 0 0";
	%basePos = VectorSub(VectorAdd(%this.getPosition(), "0 0 0.2"), "0 0 0.1");
	%group.position[%this] = %pos;
	%iter = 1;
	for(%i = 0; %i != %iter; %i++)
	{
		%brick = %group.getObject(%i);
		if(!isObject(%brick))
			continue;
		%up = %brick.getNumUpBricks();
		if(%i > 0)
		{
			%down = %brick.getNumDownBricks();
			for(%b = 0; %b < %down; %b++)
			{
				%obj = %brick.getDownBrick(%b);
				if(!isObject(%obj) || %group.isMember(%obj))
					continue;
				%pos = VectorSub(%obj.getPosition(), %basePos); //uhh
				%group.add(%obj);
				%group.position[%obj] = %pos;
				%iter++;
			}
		}
		for(%b = 0; %b < %up; %b++)
		{
			%obj = %brick.getUpBrick(%b);
			if(!isObject(%obj) || %group.isMember(%obj))
				continue;
			%pos = VectorSub(%obj.getPosition(), %basePos); //uhh
			%group.add(%obj);
			%group.position[%obj] = %pos;
			%iter++;
		}
	}
	%group.remove(%this);
	%group.position[%this] = "";
	return %group;
}

function serverCmdCaptureCell(%this)
{
	if(!isObject(%this.player))
	{
		messageClient(%this, '', "You must have a player object!");
		return;
	}
	if(!(%this.isAdmin || %this.isSuperAdmin))
	{
		messageClient(%this, '', "You must be admin.");
		return;
	}
	if(!isObject(%this.player.tempBrick))
	{
		messageClient(%this, '', "Move your ghost brick to encompass the base of the cell.");
		return;
	}
	if($Sim::Time - %this.lastCellCap < 1)
	{
		messageClient(%this, '', "\Slow down!");
		return;
	}

	initContainerBoxSearch(%this.player.tempBrick.getPosition(), "0 0 0", $TypeMasks::FxBrickObjectType);
	%brick = containerSearchNext();
	if(!isObject(%brick))
	{
		messageClient(%this, '', "Move your ghost brick to encompass the base of the cell.");
		return;
	}
	%group = %brick.CreateCellGroup();
	for(%i = 0; %i < %group.getCount(); %i++)
	{
		%b = %group.getObject(%i);
		%b.ofx = %b.getColorFXID();
		%b.setColorFX(3);
		%b.ofxset = %b.schedule(1500, setColorFX, %b.ofx);
	}
	%this.capturedCell = %group;
	%this.lastCellCap = $Sim::Time;
	messageClient(%this, '', "Captured" SPC %group.getCount() SPC "bricks.");
}

function serverCmdSaveCell(%this, %name)
{
	if(!(%this.isAdmin || %this.isSuperAdmin))
	{
		messageClient(%this, '', "You must be admin.");
		return;
	}
	if(!isObject(%this.capturedCell))
	{
		messageClient(%this, '', "Capture a cell using /captureCell first!");
		return;
	}
	// if(findWord($DungeonCellTypes, %cell) == -1)
	// {
	// 	messageClient(%this, '', "This cell type doesn't exist! Do /cellTypes to see the list. (For decor, do /saveCell style decor num");
	// 	return;
	// }
	// if(%type !$= "")
	// 	%cell = %cell @ %type;
	%file = "shape_" @ %name;
	if(isFile("saves/" @ %file @ ".bls"))
	{
		if(!%this.isSuperAdmin)
		{
			messageClient(%this, '', "Cell already exists, only SA can overwrite!");
			return;
		}
		else if(!%this.cellConfirm[%layout, %cell, %type] && %this.lastConfirm !$= %layout TAB %cell TAB %type)
		{
			messageClient(%this, '', "This cell piece already exists. Do this command again to confirm overwrite.");
			%this.cellConfirm[%layout, %cell, %type] = true;
			%this.lastconfirm = %layout TAB %cell TAB %type;
			return;
		}
		else if(%this.cellConfirm[%layout, %cell, %type])
			%this.cellConfirm[%layout, %cell, %type] = false;
	}
	%s = %this.capturedCell.saveCell(%file);
	if(!%s)
	{
		messageClient(%this, '', "Something went wrong!");
		return;
	}
	messageClient(%this, '', "\c2Saved" SPC %s SPC "bricks to" SPC %file @ "!");
	%this.capturedCell = 0;
	if(%this.lastConfirm !$= "")
	{
		%this.cellConfirm[getField(%this.lastConfirm, 0), getField(%this.lastConfirm, 1), getField(%this.lastConfirm, 2)] = false;
		%this.lastConfirm = "";
	}
}

function serverCmdSetBlock(%this, %name, %break)
{
	if(!isObject(%this.player))
		return;
	%type = findBlockData(%name);
	if(!isObject(%type))
		return;
	%bl = %this.player.getBlockAim();
	if(isObject(%bl))
	{
		if(%bl.type.name !$= "None" && !%break)
			return;
		%bl.setType(%type);
	}
}

function serverCmdSetNone(%this)
{
	if(!isObject(%this.player))
		return;
	%bl = getField(%this.player.getBlockLook(), 0);
	if(isObject(%bl))
		%bl.setType(psVoxBlockData_None);
}

function serverCmdSetRotation(%this, %r)
{
	if(!isObject(%this.player))
		return;
	%bl = getField(%this.player.getBlockLook(), 0);
	if(isObject(%bl))
		%bl.setRotation(%r);
}