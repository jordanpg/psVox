function Player::eyeCast(%this, %r, %t)
{
	%eye = %this.getEyePoint();
	%vec = VectorNormalize(%this.getEyeVector());
	%end = VectorAdd(%eye, VectorScale(%vec, %r));
	return containerRayCast(%eye, %end, %t);
}

function getRayNormal(%ray)
{
	%nX = mFloatLength(getWord(%ray, 4), 0);
	%nY = mFloatLength(getWord(%ray, 5), 0);
	%nZ = mFloor(getWord(%ray, 6));
	return %nX SPC %nY SPC %nZ;
}

function Player::getBlockLook(%this)
{
	%ray = %this.eyeCast(8, $TypeMasks::FxBrickAlwaysObjectType);
	%obj = firstWord(%ray);
	if(!isObject(%obj))
		return -1;
	if(!isObject(%obj.block))
		return -1;
	if(%obj.block.class !$= "psVoxBlock")
		return -1;
	return %obj.block TAB %ray;
}

function Player::getBlockAim(%this)
{
	%look = %this.getBlockLook();
	%block = getField(%look, 0);
	%ray = getField(%look, 1);
	if(!isObject(%block))
		return -1;
	%norm = getRayNormal(%ray);
	%relPos = %block.getRelativePos(%norm);
	%x = getWord(%relPos, 0);
	%y = getWord(%relPos, 1);
	%z = getWord(%relPos, 2);
	%rel = %block.psVox.getBlock(%x, %y, %z);
	if(!isObject(%rel))
		%rel = %block.psVox.setBlock(%x, %y, %z, psVoxBlockData_None);
	return %rel;
}

function serverCmdSelectType(%this, %n1, %n2, %n3, %n4, %n5, %n6, %n7, %n8)
{
	if(!%this.voxBuild)
		return;
	%name = trim(%n1 SPC %n2 SPC %n3 SPC %n4 SPC %n5 SPC %n6 SPC %n7 SPC %n8);
	%type = findBlockData(%name);
	if(!isObject(%type))
		return;
	%this.buildSelect = %type;
	%this.bottomPrint("\c6Selected block type" SPC %type.name @ ".", 3);
}

function serverCmdvoxBuild(%this)
{
	if(!(%this.isAdmin || %this.isSuperAdmin))
		return;
	%this.voxBuild = !%this.voxBuild;
	if(%this.voxBuild && %this.buildSelect $= "")
		%this.buildSelect = findBlockData("None");
	messageClient(%this, '', "\c6You" SPC (%this.voxBuild ? "are now in" : "are no longer in") SPC "voxel build mode.");
}

function serverCmdClearNearVox(%this, %r)
{
	if(!%this.isSuperAdmin)
		return;
	if(!isObject(%this.player))
		return;

	initContainerRadiusSearch(%this.player.getPosition(), %r, $TypeMasks::FxBrickAlwaysObjectType);
	while(isObject(%obj = containerSearchNext()))
	{
		%bl = %obj.block;
		%sc = %bl.parent;
		if(!isObject(%bl) || %done[%sc])
			continue;
		%sc.reset();
		%done[%sc] = true;
	}
}

function voxelWrenchImage::onFire(%this, %obj, %slot)
{
	%ray = %obj.getBlockLook();
	%bl = getField(%ray, 0);
	if(!isObject(%bl))
		return;

	%ray = getField(%ray, 1);
	%brick = getWord(%ray, 0);
	%pos = getWords(%ray, 1, 3);
	%bl.type.onVoxWrench(%bl, %obj, %brick);

	ServerPlay3d(wrenchHitSound, %pos);
	%p = new Projectile()
	{
		datablock		= wrenchProjectile;
		client			= %obj.client;
		initialPosition	= %pos;
		originPoint		= %pos;
		initialVelocity	= getWords(%ray, 4, 6);

		sourceObject	= %obj;
		sourceSlot		= 0;
	};
	missionCleanup.add(%p);
	%p.explode();
}

function Player::blockBreak(%this, %bl)
{
	if(isEventPending(%this.blockBreak))
		cancel(%this.blockBreak);

	if(!%this.client.voxBuild)
		return;
	%look = getField(%this.getBlockLook(), 0);
	if(isObject(%bl) && !isObject(%this.getMountedImage(0)))
	{
		if(%look.getID() != %bl.getID())
		{
			%this.breakProg = 0;
			if(isObject(%look))
			{
				%bl = %look;
				%this.blockBreak = %this.schedule(15, blockBreak, %bl);
			}
			return;
		}
		if(%bl.type.breakSpeed > 0)
			%this.breakProg++;
		if(isObject(%c = %this.client))
		{
			for(%i = 0; %i < 50; %i++)
				%str = %str @ (%i <= %this.breakProg ? "\c3" : "\c0") @ "|";
			%c.bottomPrint("<just:center>" @ %str, 1, 1, 1, 1);
		}
		if(%this.breakProg >= 50)
		{
			// echo(%bl SPC %bl.type.name SPC %bl.pos);
			%bl.type.onBreak(%bl, %this);
			%r = %bl.setType(psVoxBlockData_None);
			// echo(%r);
			%this.breakProg = 0;
			%bl = -1;
			%this.playThread(3, activate2);
		}
	}
	else
	{
		%bl = %look;
	}

	if(isObject(%bl) && %bl.type.breakSpeed > 0)
		%s = %bl.type.breakSpeed;
	else
		%s = 15;

	%this.blockBreak = %this.schedule(%s, blockBreak, %bl);
}

package voxBuild
{
	function Armor::onTrigger(%this, %obj, %slot, %val)
	{
		%r = parent::onTrigger(%this, %obj, %slot, %val);

		if(!isObject(%c = %obj.client))
			return %r;
		if(!%c.voxBuild)
			return %r;

		if(%slot $= 4 && %val)
		{
			if(isEventPending(%obj.blockBreak))
			{
				cancel(%obj.blockBreak);
				%obj.breakProg = 0;
				%bl = getField(%obj.getBlockLook(), 0);
				if(isObject(%bl))
					%bl.setRotation(%bl.rotation + (%obj.isCrouched() ? 1 : -1));
				return %r;
			}

			%look = %obj.getBlockLook();
			%lbl = getField(%look, 0);
			if(isObject(%lbl) && (%lbl.type.activated && !%obj.isCrouched()))
			{
				%ray = getField(%look, 1);
				%lbl.type.onActivated(%lbl, %obj, %ray);
				return %r;
			}

			%type = %c.buildSelect;
			%bl = %obj.getBlockAim();
			if(isObject(%bl))
			{
				if(%bl.type.name !$= "None")
					return %r;
				ServerPlay3D(BrickPlantSound, PsVox.getRealPos(%bl.pos));
				%bln = %bl.setType(%type);
				%obj.playThread(3, activate);
			}
			else
			{
				if(!isObject(groundPlane))
					return %r;
				%ray = %obj.eyeCast(8, groundPlane.getType() | $TypeMasks::FxBrickAlwaysObjectType);
				%h = firstWord(%ray);
				if(!isObject(%h) || %h.getName() !$= "groundPlane")
					return %r;
				// echo(%ray);
				%hitpos = getWords(%ray, 1, 3);
				%vPos = PsVox.getVoxPos(VectorAdd(%hitpos, "0 0 0.1"));
				// echo(%vPos);
				%vX = getWord(%vpos, 0);
				%vY = getWord(%vpos, 1);
				%vZ = getWord(%vpos, 2);
				%bln = PsVox.setBlock(%vX, %vY, %vZ, %type);
				ServerPlay3D(BrickPlantSound, PsVox.getRealPos(%bln.pos));
				%obj.playThread(3, activate);
			}
			// if(isObject(%bln))
			// 	ServerPlay3D(BrickPlantSound, PsVox.getRealPos(%bln.pos));
		}
		if(%slot $= 0 && %val)
		{
			%bl = getField(%obj.getBlockLook(), 0);
			if(isObject(%bl))
			{
				%obj.breakProg = 0;
				%obj.blockBreak(%bl);
				%obj.playThread(3, activate2);
			}
		}
		else if(%slot $= 0 && !%val)
		{
			if(isEventPending(%obj.blockBreak))
				cancel(%obj.blockBreak);
			%obj.breakProg = 0;
			%obj.playThread(3, root);
		}
		return %r;
	}
};
activatePackage(voxBuild);