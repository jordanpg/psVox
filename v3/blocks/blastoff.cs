function blastOff_New(%label)
{
	%this = new ScriptGroup("blastOff" @ (%label !$= "" ? "_" @ %label : ""))
			{
				class = "blastOff";

				input = "BLS";
				output = "BASICPHYS";

				outputReady = true;
				currAction = "";
			};
	return %this;
}

function isBlastOff(%this)
{
	if(!isObject(%this) || %this.getClassName() !$= "ScriptGroup" || %this.class !$= "blastOff")
		return false;
	return true;
}

function blastOff_NewPreload(%label, %input, %p1, %p2, %p3, %p4, %p5, %p6, %p7, %p8, %p9, %p10, %p11, %p12, %p13, %p14, %p15, %p16, %p17, %p18, %p19, %p20)
{
	%this = blastOff_New(%label);
	%e = %this.setInput(%input);
	if(%e)
		%this.input(%p1, %p2, %p3, %p4, %p5, %p6, %p7, %p8, %p9, %p10, %p11, %p12, %p13, %p14, %p15, %p16, %p17, %p18, %p19, %p20);
	return %this;
}

function blastOff::newObject(%this, %type, %params, %sid)
{
	%obj = new ScriptObject(%this.getName() @ "_obj" @ %this.getCount())
			{
				class = "blastOffObj_" @ %type;
				superClass = "blastOffObj";

				params = 0;
			};
	%this.add(%obj);
	if(%sid !$= "")
	{
		%this.specObj[%sid] = %obj;
		%obj.sid = %sid;
	}
	%ct = getFieldCount(%params);
	if(%ct > 0)
	{
		for(%i = 0; %i < %ct; %i++)
		{
			%field = getField(%params, %i);
			%name = firstWord(%field);
			%val = restWords(%field);

			%obj.setParameter(%name, %val);
		}
	}
	if(%this.debug)
		echo("blastOff : Created object" SPC %obj.getName() SPC "with" SPC %ct SPC "parameters.");
	return %obj;
}

function blastOffObj::hasParameter(%this, %name)
{
	for(%i = 0; %i < %this.params; %i++)
	{
		%p = %this.paramN[%i];
		if(%p $= %name)
			return true;
	}
	return false;
}

function blastOffObj::getParameter(%this, %name, %isindex)
{
	if(%this.paramKey[%name] $= "" || %isindex)
	{
		if(%this.paramN[%name] !$= "")
			%name = %this.paramN[%name];
		else
			return "";
	}

	return %this.paramKey[%name];
}

function blastOffObj::setParameter(%this, %name, %value)
{
	if(%this.hasParameter(%name))
	{
		%id = %this.paramIDKey[%name];
		%this.paramV[%id] = %value;
		%this.paramKey[%name] = %value;
		return true;
	}

	%this.paramN[%this.params] = %name;
	%this.paramV[%this.params] = %value;
	%this.paramKey[%name] = %value;
	%this.paramIDKey[%name] = %this.params;
	%this.params++;
	return true;
}

function blastOffObj_fxDTSBrick::getPosition(%this)
{
	if(%this.paramKeyPosition $= "")
		return "0 0 0";
	return VectorAdd(%this.paramKeyPosition, "0 0 0");
}

function blastOffObj_fxDTSBrick::getColorID(%this)
{
	if(%this.paramKeycolorID $= "")
		return 0;
	return (%this.paramKeycolorID % 63);
}

function blastOffObj_fxDTSBrick::getAngleID(%this)
{
	if(%this.paramKeyangleID $= "")
		return 0;
	return (%this.paramKeyangleID % 4);
}

function blastOffObj_fxDTSBrick::getColorFXID(%this)
{
	if(%this.paramKeycolorFXID $= "")
		return 0;
	return (%this.paramKeycolorFXID % 7);
}

function blastOffObj_fxDTSBrick::getShapeFXID(%this)
{
	if(%this.paramKeyshapeFXID $= "")
		return 0;
	return (%this.paramKeyshapeFXID % 2);
}

function blastOffObj_fxDTSBrick::isRayCasting(%this)
{
	if(%this.paramKeyRaycast $= "" || %this.paramKeyRaycast)
		return true;
	return false;
}

function blastOffObj_fxDTSBrick::isColliding(%this)
{
	if(%this.paramKeyColliding $= "" || %this.paramKeyColliding)
		return true;
	return false;
}

function blastOffObj_fxDTSBrick::isRendering(%this)
{
	if(%this.paramKeyRendering $= "" || %this.paramKeyRendering)
		return true;
	return false;
}

function blastOffObj_fxDTSBrick::getDataBlock(%this)
{
	if(%this.paramKeyDatablock $= "")
		return "";
	return %this.paramKeyDatablock;
}

function blastOff::setInput(%this, %type)
{
	if(!isFunction("blastOff_Input" @ %type))
		return false;

	%this.input = %type;
	return true;
}

function blastOff::setOutput(%this, %type)
{
	if(!isFunction("blastOff_Output" @ %type))
		return false;

	%this.output = %type;
	return true;
}

function blastOff::input(%this, %p1, %p2, %p3, %p4, %p5, %p6, %p7, %p8, %p9, %p10, %p11, %p12, %p13, %p14, %p15, %p16, %p17, %p18, %p19, %p20)
{
	if(!isFunction("blastOff_Input" @ %this.input))
		return false;

	%this.outputReady = false;
	%this.currAction = "INPUT";
	%r = call("blastOff_Input" @ %this.input, %this, %p1, %p2, %p3, %p4, %p5, %p6, %p7, %p8, %p9, %p10, %p11, %p12, %p13, %p14, %p15, %p16, %p17, %p18, %p19, %p20);
	if(%r == true)
		return true;
	echo("blastOff::input : Function returned error (" @ %r @ ")");
	%this.currAction = "";
	return false;
}

function blastOff::output(%this, %p1, %p2, %p3, %p4, %p5, %p6, %p7, %p8, %p9, %p10, %p11, %p12, %p13, %p14, %p15, %p16, %p17, %p18, %p19, %p20)
{
	if(!%this.outputReady)
		return "";
	if(!isFunction("blastOff_Output" @ %this.output))
		return "";

	%this.currAction = "OUTPUT";
	%r = call("blastOff_Output" @ %this.output, %this, %p1, %p2, %p3, %p4, %p5, %p6, %p7, %p8, %p9, %p10, %p11, %p12, %p13, %p14, %p15, %p16, %p17, %p18, %p19, %p20);
	if(firstWord(%r) $= "!ERROR!")
	{
		%this.currAction = "";
		echo("blastOff::output : Function returned error (" @ restWords(%r) @ ")");
		return "";
	}
	return %r;
}

function blastOff::outputOverride(%this, %p1, %p2, %p3, %p4, %p5, %p6, %p7, %p8, %p9, %p10, %p11, %p12, %p13, %p14, %p15, %p16, %p17, %p18, %p19, %p20)
{
	if(!isFunction("blastOff_Output" @ %this.output))
		return "";

	%this.currAction = "OUTPUT";
	%r = call("blastOff_Output" @ %this.output, %this, %p1, %p2, %p3, %p4, %p5, %p6, %p7, %p8, %p9, %p10, %p11, %p12, %p13, %p14, %p15, %p16, %p17, %p18, %p19, %p20);
	if(firstWord(%r) $= "!ERROR!")
	{
		%this.currAction = "";
		echo("blastOff::output : Function returned error (" @ restWords(%r) @ ")");
		return "";
	}
	return %r;
}

function blastOff::cancel(%this, %p1, %p2, %p3, %p4, %p5, %p6, %p7, %p8, %p9, %p10, %p11, %p12, %p13, %p14, %p15, %p16, %p17, %p18, %p19, %p20)
{
	if(%this.currAction $= "OUTPUT")
	{
		if(!isFunction("blastOff_OutputCancel" @ %this.output))
			return false;
		%r = call("blastOff_OutputCancel" @ %this.output, %this, %p1, %p2, %p3, %p4, %p5, %p6, %p7, %p8, %p9, %p10, %p11, %p12, %p13, %p14, %p15, %p16, %p17, %p18, %p19, %p20);
		return (%r == true);
	}
	if(%this.currAction $= "INPUT")
	{
		if(!isFunction("blastOff_InputCancel" @ %this.input))
			return false;
		%r = call("blastOff_InputCancel" @ %this.input, %this, %p1, %p2, %p3, %p4, %p5, %p6, %p7, %p8, %p9, %p10, %p11, %p12, %p13, %p14, %p15, %p16, %p17, %p18, %p19, %p20);
		return (%r == true);
	}
	return false;
}

function blastOff_InputBLS(%this, %f, %t)
{
	if(!isBlastOff(%this))
		return "Given object is not a valid blastOff ScriptGroup";
	if(!isFile(%f) || fileExt(%f) !$= ".bls")
		return "Requires one parameter: valid .BLS file to load";

	if(%t <= 0)
		%t = 1;

	if(%this.echo)
		echo("blastOff :" SPC %this.getName() SPC "is beginning BLS input...");
	if(isObject(%this.specObj["dataKeep"]))
	{
		%addlines = %this.specObj["dataKeep"].getParameter("lines");
		%this.specObj["dataKeep"].delete();
	}
	%data = %this.newObject("dataKeep", "", "dataKeep");
	%file = new fileObject();
	%file.openForRead(%f);
	%firstLine = %file.readLine();

	%descLines = %file.readLine();
	for(%i = 0; %i < %descLines; %i++)
		%data.setParameter("desc" @ %i, %file.readLine());
	for(%i = 0; %i < 64; %i++)
		%data.setParameter("col" @ %i, %file.readLine());
	%linect = %file.readLine();
	%data.setParameter("lines", getWord(%linect, 1) + %addlines);

	__blastOff_InputBLS_Tick(%this, %f, %t, %file);
	return true;
}

function blastOff_InputCancelBLS(%this)
{
	if(!isBlastOff(%this))
		return false;

	if(isEventPending(%this.inputTick))
		cancel(%this.inputTick);
	return true;
}

function __blastOff_InputBLS_Tick(%this, %f, %t, %file)
{
	if(isEventPending(%this.inputTick))
		cancel(%this.inputTick);

	// echo("tick");
	if(!%file.isEOF())
	{
		%lastBrick = $__blastOff_InputBLS_lastBrick__;
		%line = %file.readLine();
		// echo(0);
		if(isObject(%lastBrick) && getSubStr(%line, 0, 2) $= "+-")
		{
			// echo(4);
			%cmd = firstWord(%line);
			%data = restWords(%line);
			switch$(%cmd)
			{
				case "+-OWNER":
					%lastBrick.setParameter("bl_id", firstWord(%data));
				case "+-NTOBJECTNAME":
					%lastBrick.setParameter("name", %data);
				case "+-EMITTER":
					%endInd = strStr(%data, "\"");
					%uiname = getSubStr(%data, 0, %endInd);
					%dir = getWord(%data, getWordCount(%data) - 1);
					%lastBrick.setParameter("emitterName", %uiname);
					%lastBrick.setParameter("emitterDir", %dir);
				case "+-LIGHT":
					%endInd = strStr(%data, "\"");
					%uiname = getSubStr(%data, 0, %endInd);
					// %bluh = getWord(%data, getWordCount(%data) - 1); //idk what this thing is exactly
					%lastBrick.setParameter("light", %uiname);
				case "+-ITEM":
					%endInd = strStr(%data, "\"");
					%uiname = getSubStr(%data, 0, %endInd);
					%wordCt = getWordCount(%data);
					%pos = getWord(%data, %wordCt - 3);
					%dir = getWord(%data, %wordCt - 2);
					%resp = getWord(%data, %wordCt - 1);
					%lastBrick.setParameter("itemName", %uiname);
					%lastBrick.setParameter("itemPos", %pos);
					%lastBrick.setParameter("itemDir", %dir);
					%lastBrick.setParameter("itemRespawnTime", %resp);
				case "+-AUDIOEMITTER":
					%endInd = strStr(%data, "\"");
					%uiname = getSubStr(%data, 0, %endInd);
					%lastBrick.setParameter("music", %uiname);
				case "+-VEHICLE":
					%endInd = strStr(%data, "\"");
					%uiname = getSubStr(%data, 0, %endInd);
					%recol = getWord(%data, getWordCount(%data) - 1);
					%lastBrick.setParameter("vehicleName", %uiname);
					%lastBrick.setParameter("vehicleReColor", %recol);
				case "+-AUDIOEMITTER":
					%endInd = strStr(%data, "\"");
					%uiname = getSubStr(%data, 0, %endInd);
					%lastBrick.setParameter("music", %uiname);
				default:
					if(getField(%line, 0) $= "+-EVENT")
					{
						%data = getFields(%line, 1, 7);
						%lini = getField(%data, 0);
						%enab = getField(%data, 1);
						%inpu = getField(%data, 2);
						%dela = getField(%data, 3);
						%targ = getField(%data, 4);
						%trgn = getField(%data, 5);
						%outp = getField(%data, 6);
						%params = getFields(%line, 8, getFieldCount(%line) - 1);
						%paramct = getFieldCount(trim(%params));
						%lastBrick.setParameter("eventEnabled" @ %lini, %enab);
						%lastBrick.setParameter("eventInput" @ %lini, %inpu);
						%lastBrick.setParameter("eventDelay" @ %lini, %dela);
						%lastBrick.setParameter("eventTarget" @ %lini, %targ);
						%lastBrick.setParameter("eventTargetNT" @ %lini, %trgn);
						%lastBrick.setParameter("eventOutput" @ %lini, %outp);
						%lastBrick.setParameter("eventParams" @ %lini, %paramct);
						for(%i = 0; %i < %paramct; %i++)
							%lastBrick.setParameter("eventParam" @ %lini @ "_" @ %i, getField(%params, %i));
						%lastBrick.setParameter("eventLines", %lastBrick.getParameter("eventLines") + 1);
					}
			}
		}
		else
		{
			// echo(1);
			%lastBrick = %this.newObject("fxDTSBrick");
			%endInd = strStr(%line, "\"");
			%uiname = getSubStr(%line, 0, %endInd);
			%wordct = getWordCount(%line);
			%render = getWord(%line, %wordct - 1);
			%collide = getWord(%line, %wordct - 2);
			%raycast = getWord(%line, %wordct - 3);
			%shapefx = getWord(%line, %wordct - 4);
			%colorfx = getWord(%line, %wordct - 5);
			%print = getWord(%line, %wordct - 6);
			%colorid = getWord(%line, %wordct - 7);
			%baseplate = getWord(%line, %wordct - 8);
			%angleid = getWord(%line, %wordct - 9);
			%position = getWords(%line, %wordct - 12, %wordct - 10);
			// echo(2);
			%lastBrick.setParameter("Datablock", %uiname);
			%lastBrick.setParameter("Rendering", %render);
			%lastBrick.setParameter("Collision", %collide);
			%lastBrick.setParameter("Raycasting", %raycast);
			%lastBrick.setParameter("ShapeFXID", %shapefx);
			%lastBrick.setParameter("ColorFXID", %colorFX);
			%lastBrick.setParameter("Baseplate", %baseplate);
			%lastBrick.setParameter("AngleID", %angleid);
			%lastBrick.setParameter("Position", %position);
			%lastBrick.setParameter("ColorID", %colorID);
			%lastBrick.setParameter("Print", %print);
			%lastBrick.setParameter("eventLines", 0);
			// echo(3);
			$__blastOff_InputBLS_lastBrick__ = %lastBrick;
		}
	}
	else
	{
		%this.outputReady = true;
		%this.blsInput = true;
		if(%this.echo)
			echo("blastOff :" SPC %this.getName() SPC "has finished BLS input.");
		%this.currAction = "";
		return;
	}
	
	// echo("tock");
	%this.inputTick = schedule(%t, 0, __blastOff_InputBLS_Tick, %this, %f, %t, %file);
}

function blastOff_InputSTRING(%this, %lines)
{
	if(!isBlastOff(%this))
		return false;

	while(%lines !$= "")
	{
		%lines = nextToken(%lines, "line", "\n");
		if(isObject(%lastBrick) && getSubStr(%line, 0, 2) $= "+-")
		{
			// echo(4);
			%cmd = firstWord(%line);
			%data = restWords(%line);
			switch$(%cmd)
			{
				case "+-OWNER":
					%lastBrick.setParameter("bl_id", firstWord(%data));
				case "+-NTOBJECTNAME":
					%lastBrick.setParameter("name", %data);
				case "+-EMITTER":
					%endInd = strStr(%data, "\"");
					%uiname = getSubStr(%data, 0, %endInd);
					%dir = getWord(%data, getWordCount(%data) - 1);
					%lastBrick.setParameter("emitterName", %uiname);
					%lastBrick.setParameter("emitterDir", %dir);
				case "+-LIGHT":
					%endInd = strStr(%data, "\"");
					%uiname = getSubStr(%data, 0, %endInd);
					// %bluh = getWord(%data, getWordCount(%data) - 1); //idk what this thing is exactly
					%lastBrick.setParameter("light", %uiname);
				case "+-ITEM":
					%endInd = strStr(%data, "\"");
					%uiname = getSubStr(%data, 0, %endInd);
					%wordCt = getWordCount(%data);
					%pos = getWord(%data, %wordCt - 3);
					%dir = getWord(%data, %wordCt - 2);
					%resp = getWord(%data, %wordCt - 1);
					%lastBrick.setParameter("itemName", %uiname);
					%lastBrick.setParameter("itemPos", %pos);
					%lastBrick.setParameter("itemDir", %dir);
					%lastBrick.setParameter("itemRespawnTime", %resp);
				case "+-AUDIOEMITTER":
					%endInd = strStr(%data, "\"");
					%uiname = getSubStr(%data, 0, %endInd);
					%lastBrick.setParameter("music", %uiname);
				case "+-VEHICLE":
					%endInd = strStr(%data, "\"");
					%uiname = getSubStr(%data, 0, %endInd);
					%recol = getWord(%data, getWordCount(%data) - 1);
					%lastBrick.setParameter("vehicleName", %uiname);
					%lastBrick.setParameter("vehicleReColor", %recol);
				case "+-AUDIOEMITTER":
					%endInd = strStr(%data, "\"");
					%uiname = getSubStr(%data, 0, %endInd);
					%lastBrick.setParameter("music", %uiname);
				default:
					if(getField(%line, 0) $= "+-EVENT")
					{
						%data = getFields(%line, 1, 7);
						%lini = getField(%data, 0);
						%enab = getField(%data, 1);
						%inpu = getField(%data, 2);
						%dela = getField(%data, 3);
						%targ = getField(%data, 4);
						%trgn = getField(%data, 5);
						%outp = getField(%data, 6);
						%params = getFields(%line, 8, getFieldCount(%line) - 1);
						%paramct = getFieldCount(trim(%params));
						%lastBrick.setParameter("eventEnabled" @ %lini, %enab);
						%lastBrick.setParameter("eventInput" @ %lini, %inpu);
						%lastBrick.setParameter("eventDelay" @ %lini, %dela);
						%lastBrick.setParameter("eventTarget" @ %lini, %targ);
						%lastBrick.setParameter("eventTargetNT" @ %lini, %trgn);
						%lastBrick.setParameter("eventOutput" @ %lini, %outp);
						%lastBrick.setParameter("eventParams" @ %lini, %paramct);
						for(%i = 0; %i < %paramct; %i++)
							%lastBrick.setParameter("eventParam" @ %lini @ "_" @ %i, getField(%params, %i));
						%lastBrick.setParameter("eventLines", %lastBrick.getParameter("eventLines") + 1);
					}
			}
		}
		else
		{
			// echo(1);
			%lastBrick = %this.newObject("fxDTSBrick");
			%endInd = strStr(%line, "\"");
			%uiname = getSubStr(%line, 0, %endInd);
			%wordct = getWordCount(%line);
			%render = getWord(%line, %wordct - 1);
			%collide = getWord(%line, %wordct - 2);
			%raycast = getWord(%line, %wordct - 3);
			%shapefx = getWord(%line, %wordct - 4);
			%colorfx = getWord(%line, %wordct - 5);
			%print = getWord(%line, %wordct - 6);
			%colorid = getWord(%line, %wordct - 7);
			%baseplate = getWord(%line, %wordct - 8);
			%angleid = getWord(%line, %wordct - 9);
			%position = getWords(%line, %wordct - 12, %wordct - 10);
			// echo(2);
			%lastBrick.setParameter("Datablock", %uiname);
			%lastBrick.setParameter("Rendering", %render);
			%lastBrick.setParameter("Collision", %collide);
			%lastBrick.setParameter("Raycasting", %raycast);
			%lastBrick.setParameter("ShapeFXID", %shapefx);
			%lastBrick.setParameter("ColorFXID", %colorFX);
			%lastBrick.setParameter("Baseplate", %baseplate);
			%lastBrick.setParameter("AngleID", %angleid);
			%lastBrick.setParameter("Position", %position);
			%lastBrick.setParameter("ColorID", %colorID);
			%lastBrick.setParameter("Print", %print);
			%lastBrick.setParameter("eventLines", 0);
			%ct++;
		}
	}

	if(isObject(%this.specObj["dataKeep"]))
	{
		%addlines = %this.specObj["dataKeep"].getParameter("lines");
		%this.specObj["dataKeep"].delete();
	}
	%data = %this.newObject("dataKeep", "", "dataKeep");
	for(%i = 0; %i < 64; %i++)
		%data.setParameter("col" @ %i, getColorIDTable(%i));
	%data.setParameter("lines", %ct + %addlines);
	%this.outputReady = true;
	%this.currAction = "";
	%this.blsInput = true;
	return true;
}

function blastOff_OutputECHO(%this)
{
	if(!isBlastOff(%this))
		return "Given object is not a valid blastOff ScriptGroup";

	%ct = %this.getCount();
	echo("blastOff object" SPC %this.getName() SPC "outputting to console with" SPC %ct SPC "objects:");
	for(%i = 0; %i < %ct; %i++)
	{
		%obj = %this.getObject(%i);
		echo(%i @ ":" SPC %obj.getName() @ "," SPC %obj.params SPC "parameter(s)...");
		for(%p = 0; %p < %obj.params; %p++)
			echo("->param" SPC %p @ ":" SPC %obj.paramN[%p] SPC ":" SPC %obj.paramV[%p]);
	}
	return true;
}

//Credit to Space Guy for this
function getClosestPaintColor(%rgba)
{
	%prevdist = 100000;
	%colorMatch = 0;
	for(%i = 0;%i < 64;%i++)
	{
		%color = getColorIDTable(%i);
		if(vectorDist(%rgba,getWords(%color,0,2)) < %prevdist && getWord(%rgba,3) - getWord(%color,3) < 0.3 && getWord(%rgba,3) - getWord(%color,3) > -0.3)
		{
			%prevdist = vectorDist(%rgba,%color);
			%colormatch = %i;
		}
	}
	return %colormatch;
}

function angleToRot(%id)
{
	switch(%id)
	{
		case 0: %rotation = "1 0 0 0";
		case 1: %rotation = "0 0 1 90";
		case 2: %rotation = "0 0 1 180";
		case 3: %rotation = "0 0 -1 90";
		default: %rotation = "1 0 0 0";
	}
	return %rotation;
}

function rotateVector(%vector, %axis, %val)
{
	switch(%val)
	{
		case 1:
			%nX = getWord(%axis, 0) + (getWord(%vector, 1) - getWord(%axis, 1));
			%nY = getWord(%axis, 1) - (getWord(%vector, 0) - getWord(%axis, 0));
			%new = %nX SPC %nY SPC getWord(%vector, 2);
		case 2:
			%nX = getWord(%axis, 0) - (getWord(%vector, 0) - getWord(%axis, 0));
			%nY = getWord(%axis, 1) - (getWord(%vector, 1) - getWord(%axis, 1));
			%new = %nX SPC %nY SPC getWord(%vector, 2);
		case 3:
			%nX = getWord(%axis, 0) - (getWord(%vector, 1) - getWord(%axis, 1));
			%nY = getWord(%axis, 1) + (getWord(%vector, 0) - getWord(%axis, 0));
			%new = %nx SPC %nY SPC getWord(%vector, 2);
		default: %new = vectorAdd(%vector, %axis);
	}
	return %new;
}

function findWord(%string, %searchWord)
{
	%searchWord = firstWord(%searchWord);
	%wordCt = getWordCount(%string);
	for(%i = 0; %i < %wordCt; %i++)
	{
		%word = getWord(%string, %i);
		if(%word $= %searchWord)
			return %i;
	}
	return -1;
}

function blastOff::getNextBrick(%this, %i)
{
	%ct = %this.getCount();
	// echo("next 0");
	if(%i >= (%ct - 1))
		return -1;
	// echo("next 1");
	for(%b = (%i + 1); %b < %ct; %b++)
	{
		%obj = %this.getObject(%b);
		if(%obj.class $= "blastOffObj_fxDTSBrick")
			return %obj.getID() SPC %b;
	}
	// echo("next 2");
	return -1;
}

function blastOffObj_dataKeep::solvePaintColours(%this)
{
	for(%i = 0; %i < 64; %i++)
	{
		%col = %this.getParameter("col" @ %i);
		if(%col $= "")
			continue;
		%this.setParameter("colID" @ %i, getClosestPaintColor(%col));
	}
	%this.setParameter("solved", true);
}

function blastOff_OutputBASICPHYS(%this, %offset, %rot, %speed, %bg, %centre, %reset, %err, %float, %resolve, %brickhook, %hook0, %hook1, %hook2, %hook3, %hook4, %hook5)
{
	if(!isBlastOff(%this))
		return "!ERROR! Given object is not a valid blastOff ScriptGroup";
	if(!%this.blsInput)
		return "!ERROR! BASICPHYS output only works after BLS input";
	%data = %this.specObj["dataKeep"];
	if(!isObject(%data))
		return "!ERROR! No dataKeep object found from BLS input";

	if(%this.echo)
		echo("blastOff :" SPC %this.getName() SPC "is beginning basic physical output...");
	%this.outputReady = false;
	%this.loading = true;
	%self = %this.newObject("OutputKeep", "loadOffset 0 0 0	loadRotation 0	loadSpeed 150	brickGroup" SPC BrickGroup_888888 TAB "axis 0 0 0	ignoreErrors 0	reset 0	allowFloat 1", "OutputKeep");
	%params = 0;
	if(%offset !$= "")
	{
		%self.setParameter("loadOffset", %offset);
		%params++;
	}
	if(%rot !$= "")
	{
		if(%rot < 0)
			%self.setParameter("loadRotation", %rot + 4);
		else if(%rot > 3)
			%self.setParameter("loadRotation", %rot - 4);
		else
			%self.setParameter("loadRotation", %rot);
		%params++;
	}
	if(%speed !$= "")
	{
		%self.setParameter("loadSpeed", %speed);
		%params++;
	}
	// if(%file !$= "")
	// {
	// 	if(isFile(%file) && fileExt(%file) $= ".bls")
	// 	{
	// 		%self.setParameter("saveFile", %file);
	// 		%params++;
	// 	}
	// }
	if(isObject(%bg))
	{
		%self.setParameter("brickGroup", %bg);
		%params++;
	}
	if(%centre !$= "")
	{
		if(getWordCount(%centre) == 3)
		{
			%self.setParameter("axis", %centre);
			%params++;
		}
	}
	if(%reset !$= "")
	{
		%self.setParameter("reset", %reset);
		%params++;
	}
	if(%err !$= "")
	{
		%self.setParameter("ignoreErrors", %err);
		%params++;
	}
	if(%float !$= "")
	{
		%self.setParameter("allowFloat", %float);
		%params++;
	}
	if(%params > 0)
		%self.setParameter("loadParameters", true);
	if(isFunction(%brickhook))
	{
		%self.setParameter("brickHook", %brickhook);
		for(%i = 0; %i < 6; %i++)
			%self.setParameter("brickHookArg" @ %i, %hook[%i]);
	}

	%self.setParameter("currBrick", 0);
	%self.setParameter("currIter", 0);
	%self.setParameter("currObj", -1);

	%s = %data.getParameter("solved");
	if(!%s || (%s && %resolve))
		%data.solvePaintColours();

	__blastOff_OutputBASICPHYS_Tick(%this, %self, %data);

	return true;
}

function blastOff_OutputCancelBASICPHYS(%this)
{
	if(!isBlastOff(%this))
		return false;
	if(%this.loading)
		__blastOff_OutputBASICPHYS_Complete(%this, %this.specObjOutputKeep);
	return true;
}

function __blastOff_OutputBASICPHYS_Tick(%this, %outk, %data)
{
	if(isEventPending(%this.outputTick))
		cancel(%this.outputTick);

	if(!%this.loading)
		return;

	__blastOff_OutputBASICPHYS_Next(%this, %outk, %data);

	%this.outputTick = schedule(%outk.getParameter("loadSpeed"), 0, __blastOff_outputBASICPHYS_Tick, %this, %outk, %data);
}

function __blastOff_OutputBASICPHYS_Complete(%this, %outk)
{
	%this.loading = false;
	if(%outk.getParameter("reset"))
		%this.clear();
	%this.outputReady = true;
	if(isEventPending(%this.outputTick))
		cancel(%this.outputTick);
	if(%this.echo)
		echo("blastOff :" SPC %this.getName() SPC "has finished basic physical output.");
	%this.currAction = "";
}

function __blastOff_OutputBASICPHYS_Next(%this, %outk, %data)
{
	%brickCt = %data.getParameter("lines");
	%currBrick = %outk.getParameter("currBrick");
	if((%brickCt !$= "" && %currBrick > %brickCt))
	{
		// echo("done");
		__blastOff_OutputBASICPHYS_Complete(%this, %outk);
		return;
	}
	%currObj = %outk.getParameter("currObj");
	%next = %this.getNextBrick(%currObj);
	%brick = getWord(%next, 0);

	if(%brick == -1)
	{
		__blastOff_OutputBASICPHYS_Complete(%this, %outk);
		return;
	}
	if(!isObject(%brick))
	{
		%outk.setParameter("currBrick", %currBrick++);
		return;
	}
	%outk.setParameter("currObj", getWord(%next, 1));

	%db = %brick.getParameter("Datablock");
	%datablock = $UINameTable[%db];
	if(!isObject(%datablock))
	{
		echo("blastOff output : Missing brick datablock \"" @ %db @ "\"");
		%outk.setParameter("currBrick", %currBrick++);
		return;
	}

	%position = %brick.getParameter("Position");
	%angleID = %brick.getParameter("AngleID");
	%loadrot = %outk.getParameter("loadRotation");
	%angleID += %loadrot;
	if(%angleID > 3)
		%angleID -= 4;
	%position = rotateVector(%position, %outk.getParameter("axis"), %loadrot);
	%position = VectorAdd(%position, %outk.getParameter("loadOffset"));
	%isBaseplate = %brick.getParameter("Baseplate");
	%colorID = %brick.getParameter("ColorID");
	%colorID = %data.getParameter("colID" @ %colorID);
	%print = $PrintNameTable[%brick.getParameter("Print")];
	%client = 0;
	%bg = %outk.getParameter("brickGroup");
	if(isObject(%bg.client))
		%client = %bg.client;
	%br = new fxDTSBrick()
	{
		client = %client;
		stackBL_ID = %bg.bl_id;

		datablock = %datablock;
		position = %position;
		rotation = angleToRot(%angleID);
		colorID = %colorID;
		colorFXID = %brick.getParameter("ColorFXID");
		shapeFXID = %brick.getParameter("ShapeFXID");
		printID = %print;

		isBaseplate = %isBaseplate;
		isPlanted = true;
	};
	%val = %br.plant();
	if(!%outk.getParameter("ignoreErrors") && (!isObject(%br) || (%val > 0 && !(%val == 2 && %outk.getParameter("allowFloat")))))
	{
		%br.delete();
		%outk.setParameter("currBrick", %currBrick++);
		return;
	}
	%hook = %outk.getParameter("brickHook");
	if(isFunction(%hook))
	{
		for(%a = 0; %a < 6; %a++)
			%arg[%a] = %outk.getParameter("brickHookArg" @ %a);
		%hr = call(%hook, %br, %arg0, %arg1, %arg2, %arg3, %arg4, %arg5);
		if(!%hr)
		{
			// echo("aa" SPC %hook SPC %hr SPC %arg0 SPC %arg1);
			%br.delete();
			%outk.setParameter("currBrick", %currBrick++);
			return;
		}
	}
	%bg.add(%br);
	%br.setTrusted(true);
	if(!%brick.getParameter("Raycasting"))
		%br.setRayCasting(false);
	if(!%brick.getParameter("Collision"))
		%br.setColliding(false);
	if(!%brick.getParameter("Rendering"))
		%br.setRendering(false);

	if(%brick.hasParameter("name"))
		%br.setNTObjectName(%brick.getParameter("name"));
	if(%brick.hasParameter("emitterName"))
	{
		%ui = %brick.getParameter("emitterName");
		%direction = %brick.getParameter("emitterDir");
		if(%direction - 2 >= 0)
			%direction += %loadRot;
		if(%direction - 2 > 3)
			%direction -= 4;

		%br.setEmitterDirection(%direction);
		%emitterdb = $UINameTable_Emitters[%ui];
		if(isObject(%emitterdb))
			%br.setEmitter(%emitterdb);
	}
	if(%brick.hasParameter("light"))
	{
		%ui = %brick.getParameter("light");
		%lightdb = $UINameTable_Lights[%ui];
		if(isObject(%lightdb))
			%br.setLight(%lightdb);
	}
	if(%brick.hasParameter("itemName"))
	{
		%ui = %brick.getParameter("itemName");
		%pos = %brick.getParameter("itemPos");
		%dir = %brick.getParameter("itemDir");
		%resp = %brick.getParameter("itemRespawnTime");
		%itemdb = $UINameTable_Items[%ui];
		%br.setItemPosition(%pos);
		%br.setItemDirection(%dir);
		%br.setItemRespawnTime(%resp);
		if(isObject(%itemdb))
			%br.setItem(%itemdb);
	}
	if(%brick.hasParameter("music"))
	{
		%ui = %brick.getParameter("music");
		%musicdb = $UINameTable_Music[%ui];
		if(isObject(%musicdb))
			%br.setMusic(%musicdb);
	}
	%lines = %brick.getParameter("eventLines");
	if(%lines > 0)
	{
		// echo("evs" SPC %lines);
		for(%i = 0; %i < %lines; %i++)
		{
			// echo("ev" @ %i);
			%enabled = %brick.getParameter("eventEnabled" @ %i);
			%input = %brick.getParameter("eventInput" @ %i);
			%delay = %brick.getParameter("eventDelay" @ %i);
			%target = %brick.getParameter("eventTarget" @ %i);
			%targetnt = %brick.getParameter("eventTargetNT" @ %i);
			%output = %brick.getParameter("eventOutput" @ %i);
			// %params = %brick.getParameter("eventParams" @ %i);
			%br.eventEnabled[%i] = %enabled;
			%br.eventDelay[%i] = %delay;
			%br.eventInput[%i] = %input;
			%br.eventInputIdx[%i] = inputEvent_getInputEventIdx(%input);
			%br.eventTarget[%i] = %target;
			%br.eventTargetIdx[%i] = inputEvent_getTargetIndex("fxDTSBrick", %br.eventInputIdx[%i], %target);
			%br.eventNT[%i] = %targetnt;
			%br.eventOutput[%i] = %output;
			if(%br.eventTargetIdx[%i] == -1)
				%class = "fxDTSBrick";
			else
				%class = inputEvent_getTargetClass("fxDTSBrick", %br.eventInputIdx[%i], %br.eventTargetIdx[%i]);
			%br.eventOutputIdx[%i] = outputEvent_getOutputEventIdx(%class, %output);
			%br.eventOutputAppendClient[%i] = $OutputEvent_AppendClient[%class, %br.eventOutputIdx[%i]];
			for(%p = 0; %p < 4; %p++)
			{
				%param = %brick.getParameter("eventParam" @ %i @ "_" @ %p);
				%params = $OutputEvent_parameterList[%class, %br.eventOutputIdx[%i]];
				%paramType = getField(%params, %p);
				if(firstWord(%paramType) $= "dataBlock" && isObject(%param))
					%br.eventOutputParameter[%i, %p+1] = %param.getID();
				else
					%br.eventOutputParameter[%i, %p+1] = %param;
			}
		}
		%br.numEvents = %lines;
	}

	%outk.setParameter("currBrick", %currBrick++);
	%outk.setParameter("currIter", %outk.getParameter("currIter") + 1);
}