$PsVox::UpdateSpeed = 250;
$PsVox::UpdateSize = 100;
$PsVox::UpdateReach = 1;

function psVoxUpdater_New()
{
	if(isObject(psVoxUpdater))
		return psVoxUpdater;

	new SimSet(psVoxUpdater)
	{
		psVoxUpdater = true;
	};
	psVoxUpdater.psVoxUpdate();
	if(isObject(PsVox))
		PsVox.updater = psVoxUpdater;

	return psVoxUpdater;
}

function psVoxBlock::onUpdate(%this, %obj)
{
	//pass
}

function psVoxBlock::enqUpdate(%this)
{
	if(!isObject(psVoxUpdater))
	{
		%this.type.onUpdate(%this);
		return;
	}
	psVoxUpdater.psVoxEnqueue(%this);
}

function psVoxBlock::update(%this)
{
	// %this.type.schedule($PsVox::UpdateTick, onUpdate, %this, %this);
	// %this.type.onUpdate(%this, %this);
	%this.enqUpdate(%this);
	%this.psVox.startBoxSearch(%this.pos, $PsVox::UpdateReach);
	while((%obj = %this.psVox.boxSearchNext()) != 0)
	{
		if(%obj == -1)
			continue;

		// %obj.type.shedule(1$PsVox::UpdateTick, onUpdate, %obj, %this);
		%obj.enqUpdate();
	}
}

function SimSet::psVoxEnqueue(%this, %obj)
{
	%this.schedule(0, add, %obj);
	%this.schedule(0, bringToFront, %obj);
}

function SimSet::psVoxUpdate(%this)
{
	cancel(%this.updateTick);
	if(!%this.psVoxUpdater)
		return;

	%ct = %this.getCount();
	if(%ct > 0)
	{
		%ct = %ct - 1;
		if(%ct < $PsVox::UpdateSize)
			%end = 0;
		else
			%end = %ct - $PsVox::UpdateSize;
		for(%i = %ct; %i >= %end; %i--)
		{
			%obj = %this.getObject(%i);
			if(!isObject(%obj))
			{
				%this.remove(%obj);
				continue;
			}
			if(%obj.class $= "psVoxBlock")
				%obj.type.onUpdate(%obj);
			else
				%obj.psVoxUpdate();
			%this.remove(%obj);
		}
	}

	$PsVox::UpdateTick = %this.updateTick = %this.schedule($PsVox::UpdateSpeed, psVoxUpdate);
}