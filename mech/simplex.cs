function SimplexNoise(%seed)
{
	%g3 = "1 1 0\t-1 1 0\t1 -1 0\t-1 -1 0\t1 0 1\t-1 0 1\t1 0 -1\t-1 0 -1\t0 1 1\t0 -1 1\t0 1 -1\t0 -1 -1";
	// %p = "151\t160\t137\t91\t90\t15" TAB
	// "131\t13\t201\t95\t96\t53\t194\t233\t7\t225\t140\t36\t103\t30\t69\t142\t8\t99\t37\t240\t21\t10\t23" TAB
	// "190\t6\t148\t247\t120\t234\t75\t0\t26\t197\t62\t94\t252\t219\t203\t117\t35\t11\t32\t57\t177\t33" TAB
	// "88\t237\t149\t56\t87\t174\t20\t125\t136\t171\t168\t68\t175\t74\t165\t71\t134\t139\t48\t27\t166" TAB
	// "77\t146\t158\t231\t83\t111\t229\t122\t60\t211\t133\t230\t220\t105\t92\t41\t55\t46\t245\t40\t244" TAB
	// "102\t143\t54\t65\t25\t63\t161\t1\t216\t80\t73\t209\t76\t132\t187\t208\t89\t18\t169\t200\t196" TAB
	// "135\t130\t116\t188\t159\t86\t164\t100\t109\t198\t173\t186\t3\t64\t52\t217\t226\t250\t124\t123" TAB
	// "5\t202\t38\t147\t118\t126\t255\t82\t85\t212\t207\t206\t59\t227\t47\t16\t58\t17\t182\t189\t28\t42" TAB
	// "223\t183\t170\t213\t119\t248\t152\t2\t44\t154\t163\t70\t221\t153\t101\t155\t167\t43\t172\t9" TAB
	// "129\t22\t39\t253\t19\t98\t108\t110\t79\t113\t224\t232\t178\t185\t112\t104\t218\t246\t97\t228" TAB
	// "251\t34\t242\t193\t238\t210\t144\t12\t191\t179\t162\t241\t81\t51\t145\t235\t249\t14\t239\t107" TAB
	// "49\t192\t214\t31\t181\t199\t106\t157\t184\t84\t204\t176\t115\t121\t50\t45\t127\t4\t150\t254" TAB
	// "138\t236\t205\t93\t222\t114\t67\t29\t24\t72\t243\t141\t128\t195\t78\t66\t215\t61\t156\t180";
 
	%obj = new ScriptObject()
			{
				class = "SimplexNoise";
				p = %p;
			};
	// for(%i = 0; %i < 512; %i++)
	// 	%obj.perm[%i] = getField(%p, %i & 255);
	%ct = getFieldCount(%g3);
	for(%i = 0; %i < %ct; %i++)
		%obj.grad3[%i] = getField(%g3, %i);
	%obj.setSeed(%seed);
	return %obj;
}

function SimplexNoise::setSeed(%this, %seed)
{
	if(!%seed)
		%seed = 0;

	if(%seed $= %this.seed)
		return;

	%p = "151\t160\t137\t91\t90\t15" TAB
	"131\t13\t201\t95\t96\t53\t194\t233\t7\t225\t140\t36\t103\t30\t69\t142\t8\t99\t37\t240\t21\t10\t23" TAB
	"190\t6\t148\t247\t120\t234\t75\t0\t26\t197\t62\t94\t252\t219\t203\t117\t35\t11\t32\t57\t177\t33" TAB
	"88\t237\t149\t56\t87\t174\t20\t125\t136\t171\t168\t68\t175\t74\t165\t71\t134\t139\t48\t27\t166" TAB
	"77\t146\t158\t231\t83\t111\t229\t122\t60\t211\t133\t230\t220\t105\t92\t41\t55\t46\t245\t40\t244" TAB
	"102\t143\t54\t65\t25\t63\t161\t1\t216\t80\t73\t209\t76\t132\t187\t208\t89\t18\t169\t200\t196" TAB
	"135\t130\t116\t188\t159\t86\t164\t100\t109\t198\t173\t186\t3\t64\t52\t217\t226\t250\t124\t123" TAB
	"5\t202\t38\t147\t118\t126\t255\t82\t85\t212\t207\t206\t59\t227\t47\t16\t58\t17\t182\t189\t28\t42" TAB
	"223\t183\t170\t213\t119\t248\t152\t2\t44\t154\t163\t70\t221\t153\t101\t155\t167\t43\t172\t9" TAB
	"129\t22\t39\t253\t19\t98\t108\t110\t79\t113\t224\t232\t178\t185\t112\t104\t218\t246\t97\t228" TAB
	"251\t34\t242\t193\t238\t210\t144\t12\t191\t179\t162\t241\t81\t51\t145\t235\t249\t14\t239\t107" TAB
	"49\t192\t214\t31\t181\t199\t106\t157\t184\t84\t204\t176\t115\t121\t50\t45\t127\t4\t150\t254" TAB
	"138\t236\t205\t93\t222\t114\t67\t29\t24\t72\t243\t141\t128\t195\t78\t66\t215\t61\t156\t180";

	for(%i = 0; %i < 512; %i++)
	{
		%shift = (%i + %seed) % 512;
		%this.perm[%i] = getField(%p, %shift & 255);
	}

	%this.seed = %seed;
}

function SimplexNoise::noise2d(%this, %x, %y, %seed)
{
	if(%seed !$= "")
		%this.setSeed(%seed);
	%sq3 = mPow(3, 0.5);
	%f2 = 0.5 * (%sq3 - 1);
	%s = (%x + %y) * %f2;
	%i = mFloor(%x + %s);
	%j = mFloor(%y + %s);

	%g2 = (3 - %sq3) / 6;
	%t = (%i + %j) * %g2;
	%x0 = %x - (%i - %t);
	%y0 = %y - (%j - %t);

	if(%x0 > %y0)
	{
		%i1 = 1;
		%j1 = 0;
	}
	else
	{
		%i1 = 0;
		%j1 = 1;
	}

	%x1 = %x0 - %i1 + %g2;
	%y1 = %y0 - %j1 + %g2;
	%x2 = %x0 - 1 + 2 * %g2;
	%y2 = %y0 - 1 + 2 * %g2;

	%ii = %i & 255;
	%jj = %j & 255;
	%gi0 = %this.perm[%ii + %this.perm[%jj]] % 12;
	// echo(1 SPC %gi0);
	%gi1 = %this.perm[%ii + %i1 + %this.perm[%jj + %j1]] % 12;
	// echo(2 SPC %gi1);
	%gi2 = %this.perm[%ii + 1 + %this.perm[%jj + 1]] % 12;
	// echo(3 SPC %gi2);

	%t0 = 0.5 - %x0 * %x0 - %y0 * %y0;
	if(%t0 < 0)
		%n0 = 0;
	else
	{
		%t0 = %t0 * %t0;
		%n0 = %t0 * %t0 * simp_dot2d(%this.grad3[%gi0], %x0, %y0);
	}

	%t1 = 0.5 - %x1 * %x1 - %y1 * %y1;
	if(%t1 < 0)
		%n1 = 0;
	else
	{
		%t1  = %t1 * %t1;
		%n1 = %t1 * %t1 * simp_dot2d(%this.grad3[%gi1], %x1, %y1);
	}

	%t2 = 0.5 - %x2 * %x2 - %y2 * %y2;
	if(%t2 < 0)
		%n2 = 0;
	else
	{
		%t2 = %t2 * %t2;
		%n2 = %t2 * %t2 * simp_dot2d(%this.grad3[%gi2], %x2, %y2);
	}

	return 70 * (%n0 + %n1 + %n2);
}

function SimplexNoise::noise3d(%this, %x, %y, %z, %seed)
{
	if(%seed !$= "")
		%this.setSeed(%seed);
	%f3 = (1 / 3);
	%s = (%x + %y + %z) * %f3;
	%i = mFloor(%x + %s);
	%j = mFloor(%y + %s);
	%k = mFloor(%z + %s);
 
	%g3 = (1 / 6);
	%t = (%i + %j + %k) * %g3;
	%x0 = %x - (%i - %t);
	%y0 = %y - (%j - %t);
	%z0 = %z - (%k - %t);
 
	if(%x0 >= %y0)
	{
		if(%y0 >= %z0)
		{
			%i1 = 1;
			%j1 = 0;
			%k1 = 0;
			%i2 = 1;
			%j2 = 1;
			%k2 = 0;
		}
		else if(%x0 >= %z0)
		{
			%i1 = 1;
			%k1 = 0;
			%j1 = 1;
			%i2 = 0;
			%k2 = 1;
			%j2 = 1;
		}
		else
		{
			%i1 = 0;
			%k1 = 0;
			%j1 = 1;
			%i2 = 1;
			%k2 = 0;
			%j2 = 1;
		}
	}
	else
	{
		if(%y0 < %z0)
		{
			%i1 = 1;
			%j1 = 0;
			%k1 = 0;
			%i2 = 1;
			%j2 = 1;
			%k2 = 0;
		}
		else if(%x0 < %z0)
		{
			%i1 = 0;
			%k1 = 1;
			%j1 = 0;
			%i2 = 0;
			%k2 = 1;
			%j2 = 1;
		}
		else
		{
			%i1 = 0;
			%k1 = 1;
			%j1 = 0;
			%i2 = 1;
			%k2 = 1;
			%j2 = 0;
		}
	}
 
	%x1 = %x0 - %i1 + %g3;
	%y1 = %y0 - %j1 + %g3;
	%z1 = %z0 - %k1 + %g3;
	%x2 = %x0 - %i2 + 2*%g3;
	%y2 = %y0 - %j2 + 2*%g3;
	%z2 = %z0 - %k2 + 2*%g3;
	%x3 = %x0 - 1 + 3*%g3;
	%y3 = %y0 - 1 + 3*%g3;
	%z3 = %z0 - 1 + 3*%g3;
 
	%ii = %i & 255;
	%jj = %j & 255;
	%kk = %k & 255;
	%gi0 = %this.perm[%ii + %this.perm[%jj + %this.perm[%kk]]] % 12;
	%gi1 = %this.perm[%ii + %i1 + %this.perm[%jj + %j1 + %this.perm[%kk + %k1]]] % 12;
	%gi3 = %this.perm[%ii + 1 + %this.perm[%jj + 1 + %this.perm[%kk + 1]]] % 12;
 
	%t0 = 0.5 - %x0*x0 - %y0*%y0 - %z0*%z0;
	if(%t0 < 0)
		%n0 = 0;
	else
	{
		%t0 = %t0 * %t0;
		%n0 = %t0 * %t0 * simp_dot(%this.grad3[%gi0], %x0, %y0, %z0);
	}
 
	%t1 = 0.5 - %x1 * %x1 - %y1 * %y1 - %z1 * %z1;
	if(%t1 < 0)
		%n1 = 0;
	else
	{
		%t1 = %t1 * %t1;
		%n1 = %t1 * %t1 * simp_dot(%this.grad3[%gi3], %x1, %y1, %z1);
	}
 
	%t2 = 0.5 - %x2 * %x2 - %y2 * %y2 - %z2 * %z2;
	if(%t2 < 0)
		%n2 = 0;
	else
	{
		%t2 = %t2 * %t2;
		%n2 = %t2 * %t2 * simp_dot(%this.grad3[%gi3], %x2, %y2, %z2);
	}
 
	%t3 = 0.5 - %x3 * %x3 - %y3 * %y3 - %z3 * %z3;
	if(%t3 < 0)
		%n3 = 0;
	else
	{
		%t3 = %t3 * %t3;
		%n3 = %t3 * %t3 * simp_dot(%this.grad3[%gi3], %x3, %y3, %z3);
	}
 
	return 32 * (%n0 + %n1 + %n2 + %n3);
}

function simp_dot2d(%g, %x, %y)
{
	%g0 = getWord(%g, 0) * %x;
	%g1 = getWord(%g, 1) * %y;
	return %g0 + %g1;
}

function simp_dot(%g, %x, %y, %z)
{
	%g0 = getWord(%g, 0) * %x;
	%g1 = getWord(%g, 1) * %y;
	%g2 = getWord(%g, 2) * %z;
	return %g0 + %g1 + %g2;
}
 
// function psVox::initSimplex(%this, %seed)
// {
// 	if(isObject(%this.simplex))
// 		return %this.simplex;
 
// 	%this.simplex = SimplexNoise();
// 	if(%seed $= "")
// 		%seed = getRandomSeed();
// 	%this.simplexSeed = %seed;
// 	return %this.simplex;
// }
 
// function psVoxSubChunk::applySimplex(%this, %data, %s)
// {
// 	if(isEventPending(%this.applySimplex))
// 		cancel(%this.applySimplex);

// 	if(%s <= 0)
// 		%s = 50;

// 	%simp = %this.psVox.simplex;
// 	if(!isObject(%simp))
// 		%simp = %this.psVox.initSimplex();
// 	%seed = %this.psVox.simplexSeed;

// 	%chunk = %this.psVox.chunkSize;
// 	%frequency = 1 / 16;
// 	if(%this.currSimp $= "")
// 		%this.currSimp = "0 0 0";
// 	%xl = getWord(%this.currSimp, 0);
// 	%yl = getWord(%this.currSimp, 1);
// 	%zl = getWord(%this.currSimp, 2);
 
// 	%pos = %this.getGlobalPos(%xl SPC %yl SPC %zl);
// 	%x = getWord(%pos, 0);
// 	%y = getWord(%pos, 1);
// 	%z = getWord(%pos, 2);

// 	%density = %simp.noise(%x * %frequency, %y * %frequency, %z * %frequency);

// 	if(%density > 0)
// 		%this.setBlock(%xl, %yl, %zl, %data, 1);

// 	%xl++;
// 	if(%xl >= %chunk)
// 	{
// 		%xl = 0;
// 		%yl++;
// 	}
// 	if(%yl >= %chunk)
// 	{
// 		%yl = 0;
// 		%zl++;
// 	}
// 	if(%zl >= %chunk)
// 	{
// 		%this.currSimp = "";
// 		return;
// 	}
// 	%this.currSimp = %xl SPC %yl SPC %zl;

// 	%this.applySimplex = %this.schedule(%s, applySimplex, %data, %s);
// }

function psVox::initSimplex(%this, %seed, %freq, %iter, %persist, %low, %high, %addheight)
{
	if(isObject(%this.terrain))
		%this.terrain.delete();
	if(isObject(%this.simplex))
		%this.simplex.setSeed(%seed);
	else
		%this.simplex = SimplexNoise(%seed);

	%low = (%low >= 0 ? %low : 0);
	if(%high <= %low)
	{
		if(255 < %low)
			%high = %low + 64;
		else
			%high = 255;
	}
	%this.terrain = new ScriptObject()
					{
						class = "psVoxTerrain";
						noise = %this.simplex;
						parent = %this;

						seed = %this.simplex.seed;
						freq = (%freq > 0 ? %freq : (1 / 16));
						iter = (%iter > 0 ? %iter : 8);
						persist = (%persist > 0 ? %persist : 0.5);
						low = %low;
						high = %high;
						// scale = (%scale > 0 ? %scale : 0.02);
						add = %addheight;
					};
	return %this.terrain;
}

function psVoxTerrain::getHeight(%this, %x, %y, %seed)
{
	%maxAmp = 0;
	%amp = 1;
	%f = %this.freq;
	%noise = 0;

	for(%i = 0; %i < %this.iter; %i++)
	{
		%noise += %this.noise.noise2d(%x * %f, %y * %f, %seed) * %amp;
		%maxAmp += %amp;
		%amp *= %this.persist;
		%freq *= 2;
	}

	%noise /= %maxAmp;
	%noise = %noise * (%this.high - %this.low) / 2 + (%this.high + %this.low) / 2;
	return %noise;
}

function psVoxTerrain::getDensity(%this, %x, %y, %z, %seed, %norm, %nof)
{
	%f = (!%nof ? %this.freq : 1);
	%noise = %this.noise.noise3d(%x * %f, %y * %f, %z * %f, %seed);
	if(%norm)
		%noise = (%noise + 1) / 2;
	return %noise;
}

function simpBrick(%db, %pos, %client, %colour)
{
	%brick = new fxDTSBrick()
			{
				datablock = %db;
				position = %pos;
				rotation = "0 0 0 0";

				angleID = 0;
				colorID = %colour;
				colorFXID = 0;
				shapeFXID = 0;
				brickGroup = %client.brickGroup;
				client = %client;

				isBaseplate = true;
				isPlanted = true;
			};
	%brick.schedule(0, plant);
	%brick.brickGroup.add(%brick);
}

function psVoxTerrain::heightmapTest(%this, %minX, %minY, %maxX, %maxY, %hscale, %colorID)
{
	%i = 0;
	for(%y = %minY; %y <= %maxY; %y++)
	{
		for(%x = %minX; %x <= %maxX; %x++)
		{
			%height = %this.getHeight(%x, %y) * %hscale;

			%pos = (%x * 0.5) SPC (%y * 0.5) SPC (%height * 0.2);
			// echo(%pos);
			schedule(%i*15, 0, simpBrick, Brick1x1Data, %pos, localClientConnection, %colorID);
			%i++;
		}
	}
}

function psVoxTerrain::test3d(%this, %minX, %minY, %minZ, %maxX, %maxY, %maxZ, %colorID)
{
	%i = 0;
	%minZ += 8;
	%maxZ += 8;
	for(%z = %minZ; %z <= %maxZ; %z++)
	{
		for(%y = %minY; %y <= %maxY; %y++)
		{
			for(%x = %minX; %x <= %maxX; %x++)
			{
				%density = %this.getDensity(%x, %y, %z);
				if(%density > 0)
				{
					schedule(%i * 15, 0, simpBrick, Brick1x1FData, (%x * 0.5) SPC (%y * 0.5) SPC ((%z-8) * 0.2), localClientConnection, %colorID);
					%i++;
				}
			}
		}
	}
}

function psVoxTerrainMap::addHeight(%this, %x, %y, %scale, %seed)
{
	%this.height[%x, %y] = (%this.parent.getHeight(%x, %y, %seed) * %scale) + %this.parent.add;
}

function psVoxTerrainMap::genHeightmap(%this, %xMin, %yMin, %xMax, %yMax, %scale, %seed)
{
	// $bloog = true;
	for(%y = %yMin; %y <= %yMax; %y++)
	{
		for(%x = %xMin; %x <= %xMax; %x++)
		{
			%this.addHeight(%x, %y, %scale, %seed);
		}
	}
}

function psVoxTerrainMap::heightmapTest2(%this, %minX, %minY, %maxX, %maxY, %colorID)
{
	for(%y = %minY; %y <= %maxY; %y++)
	{
		for(%x = %minX; %x <= %maxX; %x++)
		{
			%height = %this.height[%x, %y];

			%pos = (%x * 0.5) SPC (%y * 0.5) SPC (%height * 0.2);
			// echo(%pos);
			schedule(%i*15, 0, simpBrick, Brick1x1Data, %pos, localClientConnection, %colorID);
			%i++;
		}
	}
}

function psVoxTerrainMap::addDensity(%this, %x, %y, %z, %seed, %norm, %nof)
{
	%this.cell[%x, %y, %z] = %this.parent.getDensity(%x, %y, %z, %seed, %norm, %nof);
}

function psVoxTerrainMap::genCells(%this, %minX, %minY, %minZ, %maxX, %maxY, %maxZ, %seed, %norm, %nof)
{
	for(%z = %minZ; %z <= %maxZ; %z++)
	{
		for(%y = %minY; %y <= %maxY; %y++)
		{
			for(%x = %minX; %x <= %maxX; %x++)
			{
				%this.addDensity(%x, %y, %z, %seed, %norm, %nof);
			}
		}
	}
}

function psVoxTerrainMap::terrainTest(%this, %minX, %minY, %minZ, %maxX, %maxY, %maxZ, %colorID)
{
	for(%z = %minZ; %z <= %maxZ; %z++)
	{
		for(%y = %minY; %y <= %maxY; %y++)
		{
			for(%x = %minX; %x <= %maxX; %x++)
			{
				%density = %this.cell[%x, %y, %z];
				if(%density > 0)
				{
					if(%z >= %this.height[%x, %y])
						%c = 2;
					else
						%c = 8;
					schedule(%i * 15, 0, simpBrick, Brick4xCubeData, (%x * 2) SPC (%y * 2) SPC (%z * 2), localClientConnection, %c);
					%i++;
				}
			}
		}
	}
}

function psVoxTerrainMap::debugCells(%this, %minX, %minY, %minZ, %maxX, %maxY, %maxZ, %d0, %d1)
{
	for(%z = %minZ; %z <= %maxZ; %z++)
	{
		for(%y = %minY; %y <= %maxY; %y++)
		{
			for(%x = %minX; %x <= %maxX; %x++)
			{
				%density = %this.cell[%x, %y, %z];
				if(%density > %d0 && %density < %d1)
				{
					// if(%z >= %this.height[%x, %y])
					// 	%c = 2;
					// else
					// 	%c = 8;
					schedule(%i * 15, 0, simpBrick, Brick2xCubeData, (%x * 1) SPC (%y * 1) SPC (%z * 1), localClientConnection, 8);
					%i++;
				}
			}
		}
	}
}

function psVoxTerrainMap::cellHeightmap(%this, %minX, %minY, %minZ, %maxX, %maxY, %maxZ, %grass)
{
	for(%z = %minZ; %z <= %maxZ; %z++)
	{
		for(%y = %minY; %y <= %maxY; %y++)
		{
			for(%x = %minX; %x <= %maxX; %x++)
			{
				%this.cell[%x, %y, %z] = 0;
				%h = %this.height[%x, %y];
				if(%z - 1 < %h && %z + 1 > %h)
				{
					%this.cell[%x, %y, %z] = 1;
					if(%z >= %h && %grass)
						%this.cell[%x, %y, %z] = 2;
				}
				else if(%z - 1 < %h)
					%this.cell[%x, %y, %z] = -1;
			}
		}
	}
}

function psVoxTerrain::newMap(%this, %name, %keep)
{
	if(isObject(%this.map[%name]))
	{
		if(%keep)
			return %this.map[%name];
		%this.map[%name].delete();
	}

	%map = new ScriptObject("terrainMap_" @ %name)
			{
				class = "psVoxTerrainMap";
				name = %name;
				parent = %this;
			};
	%this.map[%name] = %map;
	return %map;
}

function psVoxTerrain::terrainTest_0(%this)
{
	//phase 0: init
	%height = %this.newMap("height");
	%density0 = %this.newMap("density0");
	%density1 = %this.newMap("density1");
	%land = %this.newMap("scape");
	return 0;
}

function psVoxTerrain::terrainTest_1(%this, %minX, %minY, %minZ, %maxX, %maxY, %maxZ, %scale, %shift, %map)
{
	//phase 1: generate maps
	%height = %this.map["height"];
	%density0 = %this.map["density0"];
	%density1 = %this.map["density1"];

	%i = 0;
	%height.schedule(33, genHeightmap, %minX, %minY, %maxX, %maxY, %scale);
	%i++;
	%density0.schedule(66, genCells, %minX, %minY, %minZ, %maxX, %maxY, %maxZ, "", %map);
	%i++;
	%density1.schedule(99, genCells, %minX, %minY, %minZ, %maxX, %maxY, %maxZ, %this.seed+%shift, %map);
	%this.noise.schedule(132, setSeed, %this.seed);
	%i++;
	if(%map)
	{
		%height.schedule(132, cellHeightmap, %minX, %minY, %minZ, %maxX, %maxY, %maxZ);
		%i++;
	}
	return %i;
}

function psVoxTerrain::terrainTest_2(%this, %minX, %minY, %minZ, %maxX, %maxY, %maxZ, %cp0, %cp1)
{
	//phase 2: solve scape
	%height = %this.map["height"];
	%density0 = %this.map["density0"];
	%density1 = %this.map["density1"];
	%land = %this.map["scape"];

	echo(isObject(%height));

	// echo($bloog);
	%i = 0;
	for(%z = %minZ; %z <= %maxZ; %z++)
	{
		for(%y = %minY; %y <= %maxY; %y++)
		{
			for(%x = %minX; %x <= %maxX; %x++)
			{
				%h = %height.height[%x, %y];
				if(%z-1 > %h)
				{
					// echo("skip" SPC %x SPC %y SPC %z SPC %height);
					%land.cell[%x, %y, %z] = 0;
					continue;
				}
				else
					%land.cell[%x, %y, %z] = 1;

				%d0 = mAbs(%density0.cell[%x, %y, %z]);
				%d1 = mAbs(%density1.cell[%x, %y, %z]);
				if(%d0 > %cp0 && %d0 < %cp1 && %d1 > %cp0 && %d1 < %cp1)
				{
					// echo(%x SPC %y SPC %z);
					%land.cell[%x, %y, %z] = 0;
				}
			}
		}
	}
	return %i;
}

function psVoxTerrain::terrainTest_2_1(%this, %minX, %minY, %minZ, %maxX, %maxY, %maxZ, %cp0, %cp1)
{
	//phase 2: solve scape
	%height = %this.map["height"];
	%density0 = %this.map["density0"];
	%density1 = %this.map["density1"];
	%land = %this.map["scape"];

	echo(isObject(%height));

	// echo($bloog);
	%i = 0;
	for(%z = %minZ; %z <= %maxZ; %z++)
	{
		for(%y = %minY; %y <= %maxY; %y++)
		{
			for(%x = %minX; %x <= %maxX; %x++)
			{
				%land.cell[%x, %y, %z] = 1;
				%h = %height.height[%x, %y];
				if(%z-1 > %h)
				{
					// echo("skip" SPC %x SPC %y SPC %z SPC %height);
					%land.cell[%x, %y, %z] = 0;
					continue;
				}

				%d0 = mAbs(%density0.cell[%x, %y, %z]);
				%d1 = mAbs(%density1.cell[%x, %y, %z]);
				if(%d0 > %cp0 && %d0 < %cp1 && %d1 > %cp0 && %d1 < %cp1)
				{
					// echo(%x SPC %y SPC %z);
					%land.cell[%x, %y, %z] = 0;
				}

				// else if(%d0 > %cp1 && %d1 > %cp1)
				// 	%land.cell[%x, %y, %z] = 0;
			}
		}
	}
	return %i;
}

function psVoxTerrain::terrainTest_3(%this, %minX, %minY, %minZ, %maxX, %maxY, %maxZ)
{
	//phase 3: build
	%height = %this.map["height"];
	%land = %this.map["scape"];

	%i = 0;
	for(%z = %minZ; %z <= %maxZ; %z++)
	{
		for(%y = %minY; %y <= %maxY; %y++)
		{
			for(%x = %minX; %x <= %maxX; %x++)
			{
				%c = %land.cell[%x, %y, %z];
				if(%c)
				{
					if(%z >= %height.height[%x, %y])
						%c = 2;
					else
						%c = 8;
					schedule(%i * 15, 0, simpBrick, Brick2xCubeData, (%x * 1) SPC (%y * 1) SPC (%z * 1), localClientConnection, %c);
					%i++;
				}
			}
		}
	}
	return %i;
}

function psVoxTerrain::terrainTest(%this, %minX, %minY, %minZ, %maxX, %maxY, %maxZ, %scale, %cp0, %cp1, %shift)
{
	%i = %this.terrainTest_0();
	%i++;

	%i += %this.schedule(150, terrainTest_1, %minX, %minY, %minZ, %maxX, %maxY, %maxZ, %scale, %shift);
	%i++;

	%i += %this.schedule(300, terrainTest_2, %minX, %minY, %minZ, %maxX, %maxY, %maxZ, %cp0, %cp1);
	%i++;

	%i += %this.schedule(450, terrainTest_3, %minX, %minY, %minZ, %maxX, %maxY, %maxZ);
	echo("Finished terrain test in" SPC %i SPC "actions.");
}

function psVoxTerrain::terrainTestAlt(%this, %minX, %minY, %minZ, %maxX, %maxY, %maxZ, %scale, %cp0, %cp1, %shift)
{
	%i = %this.terrainTest_0();
	%i++;

	%i += %this.schedule(150, terrainTest_1, %minX, %minY, %minZ, %maxX, %maxY, %maxZ, %scale, %shift, 1);
	%i++;

	%i += %this.schedule(300, terrainTest_2_1, %minX, %minY, %minZ, %maxX, %maxY, %maxZ, %cp0, %cp1);
	%i++;

	%i += %this.schedule(450, terrainTest_3, %minX, %minY, %minZ, %maxX, %maxY, %maxZ);
	echo("Finished terrain test in" SPC %i SPC "actions.");
}

function psVoxGen_SimpChunk(%this, %cX, %cY, %cZ, %scale, %grass)
{
	%simp = %this.terrain;
	if(!isObject(%simp))
		return;

	%chunk = %this.getSubChunk(%cX, %cY, %cZ);
	if(!isObject(%chunk))
		return;

	%gen = %this.genQueue;
	if(!isObject(%gen))
	{
		echo("wat");
		return;
	}

	if(%scale <= 0)
		%scale = 0.02;

	%simp.newMap("scape", 1);

	%gen.addJobToBack(psVoxGen_SimpChunk_1, %this, %chunk, %scale, %grass);
}

function psVoxGen_SimpChunk_1(%this, %chunk, %scale, %grass)
{
	%simp = %this.terrain;
	if(!isObject(%simp))
		return;

	if(!isObject(%chunk))
		return;

	%map = %simp.mapscape;
	if(!isObject(%map))
		return;

	%start = %chunk.getGlobalPos("0 0 0");
	%size = %this.chunkSize - 1;
	%end = %chunk.getGlobalPos(%size SPC %size SPC %size);
	%minX = getWord(%start, 0);
	%minY = getWord(%start, 1);
	%maxX = getWord(%end, 0);
	%maxY = getWord(%end, 1);

	%map.genHeightmap(%minX, %minY, %maxX, %maxY, %scale);

	%this.genQueue.addJobToBack(psVoxGen_SimpChunk_2, %this, %chunk, %map, %start, %end, %grass);
}

function psVoxGen_SimpChunk_2(%this, %chunk, %map, %start, %end, %grass)
{
	if(!isObject(%map))
		return;

	%minX = getWord(%start, 0);
	%minY = getWord(%start, 1);
	%minZ = getWord(%start, 2);
	%maxX = getWord(%end, 0);
	%maxY = getWord(%end, 1);
	%maxZ = getWord(%end, 2);

	%map.cellHeightmap(%minX, %minY, %minZ, %maxX, %maxY, %maxZ, %grass);

	%this.genQueue.addJobToBack(psVoxGen_SimpChunk_3, %this, %chunk, %map, %start, %end);
}

function psVoxGen_SimpChunk_3(%this, %chunk, %map, %start, %end)
{
	if(!isObject(%map))
		return;

	if(!isObject(%chunk))
		return;

	%minX = getWord(%start, 0);
	%minY = getWord(%start, 1);
	%minZ = getWord(%start, 2);
	%maxX = getWord(%end, 0);
	%maxY = getWord(%end, 1);
	%maxZ = getWord(%end, 2);

	%i = 0;
	%b = 0;
	for(%z = %minZ; %z <= %maxZ; %z++)
	{
		for(%y = %minY; %y <= %maxY; %y++)
		{
			for(%x = %minX; %x <= %maxX; %x++)
			{
				%c = %map.cell[%x, %y, %z];
				if(%c == 1)
				{
					%this.schedule(mFloor(%i / 10) * 50, setBlock, %x, %y, %z, psVoxBlockData_Dirt2x, 1);
					%i++;
				}
				else if(%c == -1)
				{
					%this.schedule(mFloor(%b / 25) * 33, setBlock, %x, %y, %z, psVoxBlockData_Empty, 1);
					%b++;
				}
				else if(%c == 2)
				{
					%this.schedule(mFloor(%i / 10) * 50, setBlock, %x, %y, %z, psVoxBlockData_Grass2x, 1);
					%i++;
				}
			}
		}
	}
}

function psVox::Gen_Simplex(%this, %cX, %cY, %cZ, %scale, %grass, %addheight, %seed, %freq, %iter, %persist, %low, %high)
{
	if(!isObject(%this.terrain))
		%this.initSimplex(%seed, %freq, %iter, %persist, %low, %high, %addheight);
	if(!isObject(%this.genQueue))
		%this.initGen();
	%simp = %this.terrain;
	%gen = %this.genQueue;

	%chunk = %this.getSubChunk(%cX, %cY, %cZ);
	if(!isObject(%chunk))
		%gen.addJobToBack(psVoxGen_Chunk, %this, %cX, %cY, %cZ);

	%gen.addJobToBack(psVoxGen_SimpChunk, %this, %cX, %cY, %cZ, %scale, %grass);
}