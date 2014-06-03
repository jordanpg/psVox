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