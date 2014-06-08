function brickInBox(%this, %box, %rel, %ignoreZ, %boxobj)
{
	if(!isObject(%this) || %this.getClassName() !$= "fxDTSBrick")
		return false;

	if(%boxobj)
		%box = %box.getWorldBox();
	if(getWordCount(%box) < 6)
		return false;

	// echo(%this.isPlanted);
	%inbox = %this.getWorldBox();
	%minX = getWord(%inbox, 0);
	%minY = getWord(%inbox, 1);
	%minZ = getWord(%inbox, 2);
	%maxX = getWord(%inbox, 3);
	%maxY = getWord(%inbox, 4);
	%maxZ = getWord(%inbox, 5);
	// echo(INBOX SPC %inbox);

	if(%rel)
	{
		%pos = %this.getPosition();
		%min = getWords(%box, 0, 2);
		%max = getWords(%box, 3, 5);
		%min = VectorAdd(%min, %pos);
		%max = VectorAdd(%max, %pos);
		%box = %min SPC %max;
	}
	%bMinX = getWord(%box, 0);
	%bMinY = getWord(%box, 1);
	%bMinZ = getWord(%box, 2);
	%bMaxX = getWord(%box, 3);
	%bMaxY = getWord(%box, 4);
	%bMaxZ = getWord(%box, 5);
	// echo(BOX SPC %box);

	if(%minX < %bMinX)
		return false;
	if(%minY < %bMinY)
		return false;
	if(%minZ < %bMinZ && !%ignoreZ)
		return false;
	if(%maxX > %bMaxX)
		return false;
	if(%maxY > %bMaxY)
		return false;
	if(%maxZ > %bMaxZ && !%ignoreZ)
		return false;
	return true;
}

function brickInBoxDef(%this, %bx0, %bx1, %by0, %by1, %bz0, %bz1, %ignoreZ)
{
	if(!isObject(%this) || %this.getClassName() !$= "fxDTSBrick")
		return false;

	// echo(%this.isPlanted);
	%inbox = %this.getWorldBox();
	%minX = getWord(%inbox, 0);
	%minY = getWord(%inbox, 1);
	%minZ = getWord(%inbox, 2);
	%maxX = getWord(%inbox, 3);
	%maxY = getWord(%inbox, 4);
	%maxZ = getWord(%inbox, 5);
	// echo(INBOX SPC %inbox);

	if(%minX < %bx0)
		return false;
	if(%minY < %by0)
		return false;
	if(%minZ < %bz0 && !%ignoreZ)
		return false;
	if(%maxX > %bx1)
		return false;
	if(%maxY > %by1)
		return false;
	if(%maxZ > %bz1 && !%ignoreZ)
		return false;
	return true;
}

function pointInBox(%point, %box, %bounds)
{
	%minX = getWord(%box, 0) - %bounds;
	%minY = getWord(%box, 1) - %bounds;
	%minZ = getWord(%box, 2) - %bounds;
	%maxX = getWord(%box, 3) + %bounds;
	%maxY = getWord(%box, 4) + %bounds;
	%maxZ = getWord(%box, 5) + %bounds;
	%x = getWord(%point, 0);
	%y = getWord(%point, 1);
	%z = getWord(%point, 2);
	if(%x == %minX || %x == %maxX)
		%fX = true;
	if(%y == %minY || %y == %maxY)
		%fY = true;
	if(%z == %minZ || %z == %maxZ)
		%fZ = true;

	return !(%fX || %fY || %fZ);
}

function blastOff_BoxCheck(%this, %box, %rel, %ignoreZ)
{
	return brickInBox(%this, %box, %rel, %ignoreZ);
}