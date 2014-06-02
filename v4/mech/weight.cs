$PsVox::CaveInProb = 0;
$PsVox::CaveInProx = 3;
$PsVox::CaveInDist = 3;

function psVoxBlock::getWeight(%this)
{
	return %this.type.weight + 0;
}

// function psVoxBlock::getSupportingBlocks(%this)
// {
// 	%pos = %this.pos;
// 	%z = getWord(%pos, 2);
// 	%cZ = %z + 1;
// 	%pv = %this.psVox;
// 	%g = new SimSet();

// 	while(true)
// 	{
// 		%block = %pv.getBlock(setWord(%pos, 2, %cZ));
// 		if(%block == -1 || %block.getWeight() < 0)
// 			break;

// 		%g.add(%block);
// 		%cZ++;
// 	}
// 	return %g;
// }

// function psVoxBlock::getSupportingWeight(%this)
// {
// 	%g = %this.getSupportingBlocks();
// 	%ct = %g.getCount();
// 	if(%ct <= 0)
// 		return 0;

// 	%wt = 0;
// 	for(%i = 0; %i < %ct; %i++)
// 		%wt += %g.getObject(%i).getWeight();
// 	return %wt;
// }

function psVoxBlock::getSupport(%this)
{
	if(%this.type.stable)
		return 999999;
		
	%s = 0;
	%this.psVox.startBoxSearch(%this.pos, $PsVox::CaveInProx);
	while((%obj = %this.psVox.boxSearchNext()) != 0)
	{
		if(%obj == -1)
			continue;

		if(%obj.type.support > 0)
			%s += %obj.type.support;
	}
	return %s;
}

function psVoxBlock::fall(%this)
{
	%rel = %this.getRelative("0 0 -1");
	if(%rel.type.shapeType !$= "Empty" || !isObject(%rel))
	{
		if(%this.getProp("fall") == 2)
		{
			%this.setProp("fall", 0);
			if(!%this.getProp("caveIn"))
			{
				%this.setProp("caveInOrigin", "");
				%this.setProp("caveInPlayer", "");
				%this.setProp("caveInObj", "");
			}
		}
		return;
	}

	%this.swap(%rel);
}

function psVoxBlock::caveIn(%this, %player, %origin, %obj)
{
	if(%this.type.shapeType $= "Empty")
		return;

	if(%this.getProp("fall") > 0 || %this.getProp("caveIn"))
		return;

	if(%origin $= "")
		%origin = %this.pos;

	if(!isObject(%obj))
	{
		%obj = %this;
		%this.setProp("caveIn", true);
	}

	if(VectorDist(%this.pos, %origin) > $PsVox::CaveInDist)
		return;

	if(%this.getSupport() > 0)
		return;

	echo(%this SPC %player SPC %origin);
	%this.setProp("fall", 2);
	%this.setProp("caveInOrigin", %origin);
	%this.setProp("caveInPlayer", %player);
	%this.setProp("caveInObj", %obj);
	// %this.psVox.startBoxSearch(%this.pos, 1, %this);
	%this.schedule(0, update);
	// %this.psVox.startBoxSearch(%this.pos, $PsVox::CaveInProx);
	// while((%obj = %this.psVox.boxSearchNext()) != 0)
	// {
	// 	if(%obj == -1)
	// 		continue;

	// 	%obj.schedule(1, caveIn, %player, %origin);
	// }
}

package psVoxWeight
{
	function psVoxBlockData::onUpdate(%this, %obj)
	{
		// parent::onUpdate(%this, %obj);

		if(%this.gravity || %obj.getProp("fall"))
		{
			%rel = %obj.getRelative("0 0 -1");
			if(%rel.type.shapeType $= "Empty")
				%obj.fall();

			if(%obj.getProp("caveIn"))
			{
				// %obj.psVox.startBoxSearch(%obj.pos, 1);
				%bl = %obj.boxSearchNext();
				if(%bl != 0)
				{
					if(isObject(%bl) && %bl.type.shapeType !$= "Empty")
						%bl.schedule(0, caveIn, %obj.getProp("caveInPlayer"), %obj.getProp("caveInOrigin"), %obj.getProp("caveInObj"));
					%obj.schedule(0, update);
				}
				else
				{
					%obj.setProp("caveIn", false);
					%obj.setProp("caveInOrigin", "");
					%obj.setProp("caveInPlayer", "");
					%obj.setProp("caveInObj", "");
				}
			}
		}
	}

	function psVoxBlockData::onBreak(%this, %obj, %player)
	{
		// parent::onBreak(%this, %obj, %player);

		if(!%this.stable)
		{
			if(getRandom() <= $PsVox::CaveInProb)
			{
				%obj.caveIn(%player);
				%obj.psVox.startBoxSearch(%obj.pos, $CaveInProx, %obj);
				// while((%bl = %obj.psVox.boxSearchNext()) != 0)
				// {
				// 	if(!isObject(%bl) || %bl.type.shapeType $= "Empty")
				// 		continue;

				// 	%bl.schedule(1, caveIn, %obj.getProp("caveInPlayer"), %obj.getProp("caveInOrigin"));
				// }
			}
		}
	}
};
activatePackage(psVoxWeight);