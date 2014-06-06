$PsVox::BuilderBatchSize = 50;
$PsVox::BuilderSpeed = 250;
$PsVox::BuilderMaxRecurse = 5;

//Create and initialise the builder object.
function psVoxBuilder_New(%mode)
{
	if(isObject(psVoxBuilder))
		return psVoxBuilder;

	%this = new ScriptObject(psVoxBuilder)
			{
				mode = (%mode ? 1 : 0);
				queue = 0;
			};

	if(isObject(psVox))
		psVox.builder = %this;

	%this.tick();

	return %this;
}

//Build a batch of blocks by planting them individually.
function psVoxBuilder::buildIndBatch(%this)
{
	if(%this.queue <= 0)
		return;

	%size = $PsVox::BuilderBatchSize;
	if(%size > %this.queue)
		%size = %this.queue;

	%j = 0;
	%end = %this.queue - %size;
	for(%i = %this.queue-1; %i >= %end; %i--)
	{
		%obj = %this.queue[%i];
		%this.queue[%i] = "";
		if(!isObject(%obj))
			continue;
		%list[%j] = %obj;
		%j++;
	}
	%this.queue = %end;

	%cur = 0;
	%e = %j - 1;
	%i = 0;
	while(%j > 0)
	{
		%obj = %list[%cur];
		if(!isObject(%obj))
		{
			%cur++;
			continue;
		}

		%type = %obj.type;
		if(%type.shapeEmpty)
		{
			%list[%cur] = "";
			%j--;
			%cur++;
			continue;
		}

		%bo = %type.shape;
		if(!%bo.outputReady)
		{
			// echo("nop");
			%cur++;
			continue;
		}

		%obj.schedule(%curr * 10, _plant);
		%list[%cur] = "";
		%cur++;
		%j--;
		if(%cur > %e)
		{
			%i++;
			if(%i > $PsVox::BuilderMaxRecurse)
			{
				// echo("overcurse");
				break;
			}
			%cur = 0;
			// echo(%i);
		}
	}
}

//Build a batch of blocks by creating a blastoff shape for them (unfinished)
function psVoxBuilder::buildShapeBatch(%this)
{
	if(%this.queue <= 0)
		return;

	%size = $PsVox::BuilderBatchSize;
	if(%size > %this.queue)
		%size = %this.queue;

	%j = 0;
	%end = %this.queue - %size;
	for(%i = %this.queue-1; %i >= %end; %i--)
	{
		%obj = %this.queue[%i];
		%this.queue[%i] = "";
		if(!isObject(%obj))
			continue;
		%list[%j] = %obj;
		%j++;
	}
	%this.queue = %end;

	%shape = blastOff_New("buildBatch");
	%shape.selfdestruct = true;

	for(%i = 0; %i < %j; %i++)
	{
		%obj = %list[%j];
	}
}

//Add a block to the builder queue.
function psVoxBuilder::addBlock(%this, %obj)
{
	if(!isObject(%obj) || %obj.class !$= "psVoxBlock")
		return false;

	%type = %obj.type;
	if(%type.shapeEmpty || !isObject(%type.shape))
		return false;

	%this.queue[%this.queue] = %obj;
	%this.queue++;
	return true;
}

//tick tock helpful comment
function psVoxBuilder::tick(%this)
{
	if(isEventPending(%this.tick))
		cancel(%this.tick);

	if(%this.queue > 0)
	{
		if(%mode)
			%this.buildShapeBatch();
		else
			%this.buildIndBatch();
	}

	$PsVox::BuilderTick = %this.tick = %this.schedule($PsVox::BuilderSpeed, tick);
}

//Unfinished blastoff input for builder shapes.
function blastOff_InputpsVoxShape(%this, %obj)
{
	if(!isBlastOff(%this))
		return "Given object is not a valid blastOff ScriptGroup";
	if(!isBlastOff(%obj))
		return "Given target is not a valid blastOff ScriptGroup";

	%ct = %obj.getCount();
	for(%i = 0; %i < %ct; %i++)
	{
		%o = %obj.getObject(%i);
		%n = %o.getName();
		%o.setName("walanala");
		%new = new ScriptObject(%n : walanala);
		%this.add(%new);
		if(%new.sid !$= "")
			%this.specObj[%new.sid] = %new;
	}
}

function blastOff_OutputSUPERBASIC(%this, %offset, %rot, %bg, %brickhook, %hook0, %hook1, %hook2, %hook3, %hook4, %hook5)
{
	if(!isBlastOff(%this))
		return "!ERROR! Given object is not a valid blastOff ScriptGroup";

	%f = isFunction(%brickhook);
	%ct = %this.getCount();
	for(%i = 0; %i < %ct; %i++)
	{
		%obj = %this.getObject(%i);

		if(%obj.class !$= "blastOffObj_fxDTSBrick")
			continue;

		%db = %obj.getParameter("Datablock");
		%db = $UINameTable[%db];
		if(!isObject(%db))
			continue;

		%pos = %obj.getParameter("Position");
		%angle = (%obj.getParameter("AngleID") + %rot) % 4;
		%pos = rotateVector(%pos, "0 0 0", %rot);
		%pos = VectorAdd(%pos, %offset);

		%color = %obj.getParameter("ColorID");
		if(isObject(%bg.client))
			%client = %bg.client;
		else
			%client = 0;

		%br = new fxDTSBrick()
				{
					client = %client;
					stackBL_ID = %bg.bl_id;
					
					datablock = %db;
					position = %pos;
					rotation = angleToRot(%angle);
					colorID = %color;
					colorFXID = 0;
					shapeFXID = 0;
					printID = 0;

					isBaseplate = %db.isBaseplate;
					isPlanted = true;
				};
		%e = %br.plant();
		if(%e != 0 && %e != 2)
		{
			%br.delete();
			continue;
		}

		if(%f)
		{
			%hr = call(%brickhook, %br, %hook0, %hook1, %hook2, %hook3, %hook4, %hook5);
			if(!%hr)
			{
				%br.delete();
				continue;
			}
		}
		%bg.add(%br);
		%br.setTrusted(true);
	}
}