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
			};
	%this.queue = new SimSet(psVoxBuilderQueue);

	if(isObject(psVox))
		psVox.builder = %this;

	%this.tick();

	return %this;
}

//Build a batch of blocks by planting them individually.
function psVoxBuilder::buildIndBatch(%this)
{
	%ct = %this.queue.getCount();
	if(%ct <= 0)
		return;

	%size = $PsVox::BuilderBatchSize;
	if(%size > %ct)
		%size = %ct;

	%list = new SimSet();
	%end = %ct - %size;
	for(%i = %ct-1; %i >= %end; %i--)
	{
		%obj = %this.queue.getObject(%i);
		%this.queue.remove(%obj);
		if(!isObject(%obj))
			continue;
		%list.add(%obj);
	}

	%lct = %list.getCount();
	%cur = 0;
	%e = %lct - 1;
	%i = 0;
	while((%ct = %list.getCount()) > 0)
	{
		for(%j = %ct-1; %j >= 0; %j--)
		{
			// echo("loop");

			%obj = %list.getObject(%j);
			if(!isObject(%obj))
			{
				// echo("rong");
				continue;
			}

			%type = %obj.type;
			if(%type.shapeEmpty)
			{
				// echo("empty");
				%list.remove(%obj);
				continue;
			}

			%bo = %type.shape;
			if(!%bo.outputReady)
			{
				// echo("nop");
				continue;
			}

			// echo("plant");
			%obj.schedule(%cur * 10, _plant);
			%list.remove(%obj);
		}
		%i++;
		if(%i > $PsVox::BuilderMaxRecurse)
		{
			// echo("overcurse");
			%over = true;
			break;
		}
	}
	if(%over)
	{
		%ct = %list.getCount();
		for(%i = 0; %i < %ct; %i++)
			%this.queue.add(%list.getObject(%i));
	}
	%list.delete();

	if(%this.queue.getCount() == 0)
		%this.onFinishedBuilding();
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

	if(%obj.type.shapeEmpty || !isObject(%obj.type.shape))
		return false;

	%this.queue.add(%obj);
	return true;
}

//tick tock helpful comment
function psVoxBuilder::tick(%this)
{
	if(isEventPending(%this.tick))
		cancel(%this.tick);

	if(%this.queue.getCount() > 0)
	{
		if(%mode)
			%this.buildShapeBatch();
		else
			%this.buildIndBatch();
	}

	$PsVox::BuilderTick = %this.tick = %this.schedule($PsVox::BuilderSpeed, tick);
}

function psVoxBuilder::onFinishedBuilding(%this)
{
	//pass
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

function blastOff_OutputSUPERBASIC(%this, %offset, %rot, %bg, %brickhook, %hook0, %hook1, %hook2, %hook3, %hook4, %hook5, %hook7, %hook8, %hook9)
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
			%hr = call(%brickhook, %br, %hook0, %hook1, %hook2, %hook3, %hook4, %hook5, %hook7, %hook8, %hook9);
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