$PsVox::BuilderBatchSize = 50;
$PsVox::BuilderSpeed = 250;
$PsVox::BuilderMaxRecurse = 5;

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
		if(%type.shapeType $= "Empty")
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

function psVoxBuilder::addBlock(%this, %obj)
{
	if(!isObject(%obj) || %obj.class !$= "psVoxBlock")
		return false;

	%type = %obj.type;
	if(%type.shapeType $= "Empty" || !isObject(%type.shape))
		return false;

	%this.queue[%this.queue] = %obj;
	%this.queue++;
	return true;
}

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