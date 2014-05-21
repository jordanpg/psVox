$PsVox::CaveInProb = (1 / 8);
$PsVox::CaveInProx = 5;
$PsVox::CaveInDist = 5;

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
	if(%this.data.stable)
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
			%this.setProp("fall", 0);
		return;
	}

	%this.swap(%rel);
}

function psVoxBlock::caveIn(%this, %player, %origin)
{
	if(%this.getProp("fall"))
		return;

	if(%this.getSupport() > 0)
		return;

	if(%origin $= "")
		%origin = %this.pos;

	if(VectorDist(%this.pos, %origin) > $PsVox::CaveInDist)
		return;

	%this.setProp("fall", 2);
	%this.update();
	%this.psVox.startBoxSearch(%this.pos, $PsVox::CaveInProx);
	while((%obj = %this.psVox.boxSearchNext()) != 0)
	{
		if(%obj == -1)
			continue;

		%obj.schedule(0, caveIn, %player, %origin);
	}
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
		}
	}

	function psVoxBlockData::onBreak(%this, %obj, %player)
	{
		// parent::onBreak(%this, %obj, %player);

		if(!%this.stable)
		{
			if(getRandom() <= $PsVox::CaveInProb)
				%obj.caveIn(%player);
		}
	}
};
activatePackage(psVoxWeight);