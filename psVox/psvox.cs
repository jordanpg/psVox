$PsVox::BlockPlantRetry = 150;

//Create the psVox object.
function psVox_New(%brickGroup, %cell, %chunk)
{
	if(isObject(PsVox))
		return PsVox;
	if(!isObject(%brickGroup) || %brickGroup.getClassName() !$= "SimGroup")
		%brickGroup = BrickGroup_888888;
	%chunk = mFloor(%chunk);
	$PSVOX = new ScriptGroup(PsVox)
			{
				blockGroup = psVoxBlockGroup_New();
				brickGroup = %brickGroup;
				updater = psVoxUpdater_New();
				builder = psVoxBuilder_New();

				chunkSize = (%chunk > 0 ? %chunk : 16);
				cellSize = (%cell >= 0.5 ? %cell : 2);
			};
	//Initialise the generation queue system.
	PsVox.initGen();
	return $PSVOX;
}

//Create the psVoxBlockData group.
function psVoxBlockGroup_New()
{
	if(isObject(psVoxBlockGroup))
		return psVoxBlockGroup;
	$PVBLOCKGROUP = new SimGroup(psVoxBlockGroup)
					{
						//Default none type.
						new ScriptObject(psVoxBlockData_None)
						{
							class = "psVoxBlockData";
							name = "None";

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

							breakSpeed = -1;
							weight = -1;
							support = 0;
						};
					};
	//Make sure psVox knows we exist.
	if(isObject(PsVox))
		PsVox.blockGroup = $PVBLOCKGROUP;

	return $PVBLOCKGROUP;
}

//Searches by name for a block data object.
function findBlockData(%name)
{
	if(!isObject(psVoxBlockGroup))
		return -1;
	%ct = psVoxBlockGroup.getCount();
	for(%i = 0; %i < %ct; %i++)
	{
		%data = psVoxBlockGroup.getObject(%i);
		if(%data.name $= %name)
			return %data;
	}
	return -1;
}

//Adds psVoxBlockData objects to the block group automatically.
function psVoxBlockData::onAdd(%this)
{
	if(!isObject(psVoxBlockGroup))
		psVoxBlockGroup_New();
	psVoxBlockGroup.add(%this);
}

//Callback for when a block in a subChunk is set to a data type.
function psVoxBlockData::onBlockSet(%this, %obj, %overrideKeep, %noupdate)
{
	%obj.transistion();
	if(!%this.carryProps && !%overrideKeep)
	{
		// echo("asff" SPC %obj SPC %this.name SPC %overrideKeep);
		%obj.props.clearTags();
	}
	if(!%noupdate)
		%obj.schedule(0, update);
}

//Callback for when a block is rotated.
function psVoxBlockData::onBlockRotate(%this, %obj, %r, %noupdate)
{
	if(%this.rotates)
		%obj.transistion();
	if(!%noupdate)
		%obj.schedule(0, update);
}

//Callback for when the voxel wrench is used on a block.
function psVoxBlockData::onVoxWrench(%this, %block, %obj, %brick, %noupdate)
{
	//Naffin' here.
	if(!%noupdate)
		%obj.schedule(0, update);
}

//Callback for when a player activates a block.
function psVoxBlockData::onActivated(%this, %block, %obj, %ray, %noupdate)
{
	//Naffin' here oither.
	if(!%noupdate)
		%obj.schedule(0, update);
}

//Used to get a chunk position from a voxel world position.
function psVox::getChunkPos(%this, %pos)
{
	%x = getWord(%pos, 0);
	%y = getWord(%pos, 1);
	%z = getWord(%pos, 2);
	%x = mFloor(%x / %this.chunkSize);
	%y = mFloor(%y / %this.chunkSize);
	if(%z !$= "")
		%z = " " @ mFloor(%z / %this.chunkSize);
	return %x SPC %y @ %z;
}

//Used to get a Torque space position from a voxel world position.
function psVox::getRealPos(%this, %vPos)
{
	%rPos = VectorScale(%vPos, %this.cellSize);
	%rPos = VectorAdd(%rPos, "0 0" SPC %this.cellSize / 2);
	return %rPos;
}

//Used to get a voxel world position from a Torque space position.
function psVox::getVoxPos(%this, %rPos)
{
	// %rX = mFloor(getWord(%rPos, 0));
	// %rY = mFloor(getWord(%rPos, 1));
	// %rZ = mFloor(getWord(%rPos, 2));
	// %rPos = %rX SPC %rY SPC %rZ;
	%hCell = %this.cellSize / 2;
	%rPos = VectorAdd(%rPos, %hCell SPC %hCell SPC 0);
	%vPos = VectorScale(%rPos, (1 / %this.cellSize));
	%x = mFloor(getWord(%vPos, 0));
	%y = mFloor(getWord(%vPos, 1));
	%z = mFloor(getWord(%vPos, 2));
	return %x SPC %y SPC %z;
}

//Used to get a voxel world position from a local sub chunk position.
function psVoxSubChunk::getGlobalPos(%this, %pos)
{
	%psVox = %this.psVox;
	%x = getWord(%pos, 0);
	%y = getWord(%pos, 1);
	%z = getWord(%pos, 2);
	%gCX = getWord(%this.pos, 0);
	%gCY = getWord(%this.pos, 1);
	%gCZ = getWord(%this.pos, 2);
	%gX = %gCX * %psVox.chunkSize;
	%gY = %gCY * %psVox.chunkSize;
	%gZ = %gCZ * %psVox.chunkSize;
	return (%x + %gX) SPC (%y + %gY) SPC (%z + %gZ);
}

//Create a new chunk object.
function psVox::createChunk(%this, %cX, %cY, %preload)
{
	if(%preload $= "")
		%preload = "0 0";
	%c = %this.getChunk(%cX, %cY);
	if(!isObject(%c))
	{
		%c = new ScriptGroup("PsVoxChunk" @ %cX @ "_" @ %cY)
			{
				class = "psVoxChunk";
				parent = %this;

				pos = %cX SPC %cY;

				zMin = 0;
				zMax = 0;
			};
		%this.add(%c);
		%this.chunk[%cX, %cY] = %c;
	}
	if(getWordCount(%preload) >= 2)
	{
		%min = (getWord(%preload, 0) + 0);
		%max = (getWord(%preload, 1) + 0);
		for(%i = %min; %i <= %max; %i++)
		{
			%sc = %c.getSubChunk(%i);
			if(!isObject(%sc))
				%c.createSubChunk(%i);
		}
	}

	if(%cX < %this.xMin || %this.xMin $= "")
		%this.xMin = %cX;
	else if(%cX > %this.xMax || %this.xMax $= "")
		%this.xMax = %cX;
	if(%cY < %this.yMin || %this.yMin $= "")
		%this.yMin = %cY;
	else if(%cY > %this.yMax || %this.yMax $= "")
		%this.yMax = %cY;

	return %c;
}

//Used to create an area of chunks at once.
function psVox::createChunks(%this, %range, %preload)
{
	%cXMin = getWord(%range, 0);
	%cYMin = getWord(%range, 1);
	%cXMax = getWord(%range, 2);
	%cYMax = getWord(%range, 3);
	for(%y = %cYMin; %y <= %cYMax; %y++)
	{
		for(%x = %cXMin; %x <= %cXMax; %x++)
			%list = %list TAB %this.createChunk(%x, %y, %preload);
	}
	return trim(%list);
}

//Get the chunk at the given chunk position.
function psVox::getChunk(%this, %cX, %cY)
{
	%c = %this.chunk[%cX, %cY];
	if(!isObject(%c))
		return -1;
	return %c;
}

//Create a sub chunk within a chunk.
function psVoxChunk::createSubChunk(%this, %cZ, %type)
{
	%c = %this.getSubChunk(%cZ);
	if(isObject(%c))
		return %c;
	%upos = strReplace(%this.pos, " ", "_");
	%c = new ScriptGroup("PsVoxSubChunk" @ %upos @ "_" @ %cZ)
		{
			class = "psVoxSubChunk";
			parent = %this;
			psVox = %this.parent;

			pos = %this.pos SPC %cZ;
			init = 0;
		};
	%c.init(%type);
	%this.add(%c);
	%this.subChunk[%cZ] = %c;
	%this.parent.subChunk[%upos, %cZ] = %c;

	if(%cZ < %this.zMin)
		%this.zMin = %cZ;
	else if(%cZ > %this.zMax)
		%this.zMax = %cZ;
	return %c;
}

//Get a sub chunk within a chunk.
function psVoxChunk::getSubChunk(%this, %cZ)
{
	%c = %this.subChunk[%cZ];
	if(!isObject(%c))
		return -1;
	return %c;
}

//Get a sub chunk at the given XYZ chunk position.
function psVox::getSubChunk(%this, %cX, %cY, %cZ)
{
	%c = %this.subChunk[%cX, %cY, %cZ];
	if(!isObject(%c))
		return -1;
	return %c;
}

//Create a sub chunk at the given XYZ position.
function psVox::createSubChunk(%this, %cX, %cY, %cZ, %type)
{
	%sc = %this.getSubChunk(%cX, %cY, %cZ);
	if(isObject(%sc))
		return %sc;

	%c = %this.getChunk(%cX, %cY);
	if(!isObject(%c))
		%c = %this.createChunk(%cX, %cY);

	%sc = %c.createSubChunk(%cZ, %type);
	return %sc;
}

//Initialise the sub chunk by filling it with blocks.
function psVoxSubChunk::init(%this, %type)
{
	if(%this.init)
		return false;
	%size = %this.psVox.chunkSize;
	for(%z = 0; %z < %size; %z++)
	{
		for(%y = 0; %y < %size; %y++)
		{
			for(%x = 0; %x < %size; %x++)
				%this.createBlock(%x, %y, %z, %type);
		}
	}
	%this.init = true;
	return true;
}

//Create a block at the given local position in the sub chunk.
function psVoxSubChunk::createBlock(%this, %x, %y, %z, %data)
{
	%size = %this.psVox.chunkSize;
	if(%x < 0 || %x >= %size)
		return -1;
	if(%y < 0 || %y >= %size)
		return -1;
	if(%z < 0 || %z >= %size)
		return -1;
	if(!isObject(%data) || (%data.class !$= "psVoxBlockData" && %data.superClass !$= "psVoxBlockData"))
		%data = psVoxBlockData_None;
	%block = %this.getBlock(%x, %y, %z);
	if(isObject(%block))
		return %this.setBlock(%x, %y, %z, %data);

	%upos = strReplace(%this.pos, " ", "_");
	%block = new ScriptObject("psVoxBlock_" @ %data.name)
			{
				class = "psVoxBlock";
				parent = %this;
				psVox = %this.psVox;

				cPos = %this.pos;
				lPos = %x SPC %y SPC %z;
				pos = %this.getGlobalPos(%x SPC %y SPC %z);
				rotation = 0;
				type = %data;
			};
	%bricks = new SimSet()
				{
					className = "psVoxBricks";
				};
	%block.bricks = %bricks;
	%props = new ScriptObject()
				{
					class = "psVoxProps";
					parent = %block;

					tags = 0;
				};
	%block.props = %props;

	%this.add(%block);
	%this.block[%x, %y, %z] = %block;
	return %block;
}

//Set the block at the given local position to a different type.
function psVoxSubChunk::setBlock(%this, %x, %y, %z, %data, %noupdate, %carryProps)
{
	if(%data.class !$= "psVoxBlockData" && %data.superClass !$= "psVoxBlockData")
		return -1;
	%b = %this.getBlock(%x, %y, %z);
	if(isObject(%b))
	{
		if(%b.type.getID() == %data.getID())
			return %b;
		%b.setName("patawatakamalamadingdong");
		%new = new ScriptObject("psVoxBlock_" @ %data.name : patawatakamalamadingdong)
				{
					type = %data;
				};
		// echo(%new.getID());
		// echo(%b.getID());
		%b.schedule(33, delete);
		%this.add(%new);
		%this.block[%x, %y, %z] = %new;
	}
	else
		%new = %this.createBlock(%x, %y, %z, %data);
	if(isObject(%new))
		%data.onBlockSet(%new, %carryProps, %noupdate);
	return %new;
}

// function psVoxBlock::setTo(%this, %block)
// {
// 	if(!isObject(%block) || %block.class !$= "psVoxBlock")
// 		return false;

// 	%org = %block.getName();
// 	%block.setName("walawalawoo");
// 	%new = new ScriptObject("psVoxBlock_" @ %block.type.name : walawalawoo)
// 				{
// 					cPos = %this.cPos;
// 					lPos = %this.lPos;
// 					pos = %this.pos;
// 				};
// 	%block.setName(%org);
// 	%this.parent.add(%new);
// 	%this.parent.block[strReplace(%new.pos, " ", "_")] = %new;
// 	%this.delSched = %this.schedule(33, delete);
// 	%new.type.onBlockSet(%new, 1);
// 	return true;
// }

//Swap two blocks.
function psVoxBlock::swap(%this, %block)
{
	if(!isObject(%block) || %block.class !$= "psVoxBlock")
		return false;

	// %r1 = %block.setTo(%this);
	// %r2 = %this.setTo(%block);
	// echo(%this SPC %block SPC %r1 SPC %r2);

	%pv = %this.psVox;
	%tpos = %this.pos;
	%bpos = %block.pos;
	%ttype = %this.type;
	%btype = %block.type;
	%tnew = %pv.setBlock(getWord(%tpos,0), getWord(%tpos,1), getWord(%tpos,2), %btype, 0, 1);
	%bnew = %pv.setBlock(getWord(%bpos,0), getWord(%bpos,1), getWord(%bpos,2), %ttype, 0, 1);
	%tprop = %tnew.props;
	%bprop = %bnew.props;
	%tnew.props = %bprop;
	%bnew.props = %tprop;
	return true;
}

//Set a block within a chunk.
function psVoxChunk::setBlock(%this, %x, %y, %z, %data, %noup, %carry)
{
	%cZ = mFloor(%z / %this.parent.chunkSize);
	%sc = %this.createSubChunk(%cZ);
	if(!isObject(%sc))
		return -1;
	return %sc.setBlock(%x, %y, (%z % 16), %data, %noup, %carry);
}

//Set a block at the given voxel world position.
function psVox::setBlock(%this, %x, %y, %z, %data, %noup, %carry)
{
	%cpos = %this.getChunkPos(%x SPC %y SPC %z);
	%cX = getWord(%cpos, 0);
	%cY = getWord(%cpos, 1);
	%cZ = getWord(%cpos, 2);
	%sc = %this.createSubChunk(%cX, %cY, %cZ);
	if(!isObject(%sc))
		return -1;
	return %sc.setBlock((%x % 16), (%y % 16), (%z % 16), %data, %noup, %carry);
}

//Get the block at the given local position.
function psVoxSubChunk::getBlock(%this, %x, %y, %z)
{
	%block = %this.block[%x, %y, %z];
	if(!isObject(%block))
		return -1;
	return %block;
}

//Get the block at the given position within the chunk.
function psVoxChunk::getBlock(%this, %x, %y, %z)
{
	%cZ = mFloor(%z / %this.parent.chunkSize);
	%sc = %this.getSubChunk(%cZ);
	if(!isObject(%sc))
		return -1;

	%block = %sc.getBlock(%x, %y, (%z % 16));
	if(!isObject(%block))
		return -1;
	return %block;
}

//Get the block at the given voxel world position.
function psVox::getBlock(%this, %x, %y, %z)
{
	%cpos = %this.getChunkPos(%x SPC %y SPC %z);
	%cX = getWord(%cpos, 0);
	%cY = getWord(%cpos, 1);
	%cZ = getWord(%cpos, 2);
	%sc = %this.getSubChunk(%cX, %cY, %cZ);
	if(!isObject(%sc))
		return -1;

	%block = %sc.getBlock((%x % 16), (%y % 16), (%z % 16));
	if(!isObject(%block))
		return -1;
	return %block;
}

//Resets all blocks within the chunk to the None type.
function psVoxSubChunk::reset(%this)
{
	if(isEventPending(%this.reset))
		cancel(%this.reset);
	if(!%this.resetting)
	{
		%this.resetting = true;
		%this.unchecked = 4095;
	}
	%to = %this.unchecked - 512;
	if(%to < 0)
		%to = 0;
	for(%i = %this.unchecked; %i >= %to; %i--)
	{
		%obj = %this.getObject(%i);
		if(!isObject(%obj))
			continue;
		%r = %obj.schedule(%t * 5, setType, psVoxBlockData_None, 1);
		%this.unchecked--;
		%t++;
	}
	if(%this.unchecked > 0)
		%this.reset = %this.schedule(1, reset);
	else
		%this.resetting = false;
}

//Blastoff hook for planting blocks.
function blastOffHook_psVoxCheck(%this, %box, %block)
{
	%in = brickInBox(%this, %box, 0, 0);
	if(%in)
	{
		%block.bricks.add(%this);
		%this.block = %block;
	}
	return %in;
}

//Set the block's rotation between zero and three.
function psVoxBlock::setRotation(%this, %r)
{
	if(%r > 3 || %r < 0)
		%r = %r % 4;
	if(%r == %this.rotation)
		return %r;
	%this.rotation = %r;
	%this.type.onBlockRotate(%this, %r);
	return %r;
}

//Set the block at this position to a different type.
function psVoxBlock::setType(%this, %type, %noup)
{
	%x = getWord(%this.lPos, 0);
	%y = getWord(%this.lPos, 1);
	%z = getWord(%this.lPos, 2);
	return %this.parent.setBlock(%x, %y, %z, %type, %noup);
}

//Clear all bricks the block has created.
function psVoxBlock::clearBricks(%this)
{
	if(isEventPending(%this.clearTick))
		cancel(%this.clearTick);
	%bricks = %this.bricks;
	%ct = %bricks.getCount();
	%final = %ct - 100;
	if(%final < 0)
		%final = 0;
	for(%i = %bricks.getCount(); %i > %final; %i--)
	{
		%b = %bricks.getObject(%i);
		%b.schedule(0, delete);
	}
	if(%bricks.getCount() > 0)
		%this.clearTick = %this.schedule(1, clearBricks);
}

//Transition the block to a new shape.
function psVoxBlock::transistion(%this)
{
	if(isEventPending(%this.clearTick))
		cancel(%this.clearTick);
	%bricks = %this.bricks;
	%ct = %bricks.getCount();
	if(%ct > 0)
	{	
		%final = %ct - 100;
		if(%final < 0)
			%final = 0;
		for(%i = %bricks.getCount()-1; %i >= %final; %i--)
		{
			%b = %bricks.getObject(%i);
			if(!isObject(%b))
				continue;
			%b.schedule(0, delete);
		}
	}
	// echo(%bricks.getCount());
	if(%bricks.getCount() > 0)
		%this.clearTick = %this.schedule(50, transistion);
	else
		%this.plant();
}

//Enqueue the block in the builder, or do a local retry until it can be planted.
function psVoxBlock::plant(%this)
{
	if(isEventPending(%this.retry))
		cancel(%this.retry);

	if(%this.type.shapeType $= "Empty" || !isObject(%this.type.shape))
		return false;

	if(isObject(%this.psVox.builder))
		return %this.psVox.builder.addBlock(%this);
	else
		%r = %this._plant();

	if(%r == -1)
		%this.retry = %this.schedule($PsVox::BlockPlantRetry, plant);
}

//Actually plants the block.
function psVoxBlock::_plant(%this)
{
	// echo(p);
	%type = %this.type;
	if(%type.shapeType $= "Empty")
		return false;
	%bo = %type.shape;
	if(!isObject(%bo))
		return false;
	if(!%bo.outputReady && !isObject(%this.psVox.builder))
		return -1;

	%cell = %this.psVox.cellSize;
	%hCell = %cell / 2;
	%rPos = VectorAdd(VectorScale(%this.pos, %cell), "0 0" SPC (!%type.posFromBottom ? %hCell : 0));
	%rX = getWord(%rPos, 0);
	%rY = getWord(%rPos, 1);
	%rZ = getWord(%rPos, 2);

	%xMin = %rX - %hCell;
	%yMin = %rY - %hCell;
	%zMin = %rZ - (!%type.posFromBottom ? %hCell : 0);
	%xMax = %rX + %hCell;
	%yMax = %rY + %hCell;
	%zMax = %rZ + (!%type.posFromBottom ? %hCell : %cell);
	%box = %xMin SPC %yMin SPC %zMin SPC %xMax SPC %yMax SPC %zMax;

	if(isObject(%bo))
	{
		%bo.setOutput("BASICPHYS");
		%bo.output(%rPos, %this.rotation, -1, %this.psVox.brickGroup, "", false, true, true, false, "blastOffHook_psVoxCheck", %box, %this);
		return true;
	}
}

//Get a position relative to the block.
function psVoxBlock::getRelativePos(%this, %rel)
{
	return VectorAdd(%this.pos, %rel);
}

//Get a block relative to this one.
function psVoxBlock::getRelative(%this, %rel)
{
	%pos = %this.getRelativePos(%rel);
	%x = getWord(%pos, 0);
	%y = getWord(%pos, 1);
	%z = getWord(%pos, 2);
	return %this.psVox.getBlock(%x, %y, %z);
}

//Check properties for a given tag.
function psVoxProps::hasTag(%this, %name, %c)
{
	%name = firstWord(%name);
	if(%this.tagi[%name] !$= "")
		return true;

	if(%c)
	{
		for(%i = 0; %i < %this.tags; %i++)
		{
			if(%this.tag[%i] $= %name)
				return true;
		}
	}

	return false;
}

//Add a tag to props.
function psVoxProps::addTag(%this, %name)
{
	%name = firstWord(%name);
	if(%this.hasTag(%name))
		return false;

	%this.tag[%this.tags] = %name;
	%this.tagi[%name] = %this.tags;
	%this.tagv[%this.tags] = "";
	%this.tags++;
	return true;
}

//Set a tag value.
function psVoxProps::setTag(%this, %name, %val)
{
	%name = firstWord(%name);
	if(!%this.hasTag(%name))
		%this.addTag(%name);

	%i = %this.tagi[%name];
	%this.tagv[%i] = %val;
	return true;
}

//Get a tag value.
function psVoxProps::getTag(%this, %name)
{
	%name = firstWord(%name);
	if(!%this.hasTag(%name))
		return "";

	%i = %this.tagi[%name];
	return %this.tagv[%i];
}

//Clear all tags.
function psVoxProps::clearTags(%this)
{
	for(%i = %this.tags-1; %i >= 0; %i--)
	{
		%n = %this.tag[%i];
		%this.tagi[%n] = "";
		%this.tagv[%i] = "";
		%this.tag[%i] = "";
	}
	%this.tags = 0;
}

//Get the given tag from the block.
function psVoxBlock::getProp(%this, %name)
{
	return %this.props.getTag(%name);
}

//Set the given tag on the block.
function psVoxBlock::setProp(%this, %name, %val)
{
	return %this.props.setTag(%name, %val);
}