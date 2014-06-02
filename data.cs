datablock itemData(voxelWrenchItem : wrenchItem)
{
	category = "Weapon";
	uiName = "Voxel Wrench";
	image = voxelWrenchImage;
	colorShiftColor = "0.3 0.3 0.3 1.";
};

datablock shapeBaseImageData(voxelWrenchImage : wrenchImage)
{	
	item = voxelWrenchItem;
	projectile = wrenchProjectile;
	colorShiftColor = "0.3 0.3 0.3 1";
	showBricks = 0;
};