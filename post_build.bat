if not exist %2%3\glfw3.dll (
	copy /y %1thirdparty\glfw3.dll %2%3 > nul
	echo glfw3.dll
)
if not exist %2%3\soft_oal.dll (
	copy /y %1thirdparty\soft_oal.dll %2%3 > nul
	echo soft_oal.dll
)
if not exist %1..\..\core.zip (
	if not exist %1..\..\core mkdir %1..\..\core
	if not exist %1..\..\core\shaders mkdir %1..\..\core\shaders
	if not exist %1..\..\core\shaders\ds_opaque.glsl (
		copy /y %1fallback\ds_opaque.glsl %1..\..\core\shaders\ > nul
		echo core/shaders/ds_opaque.glsl
	)
	if not exist %1..\..\core\shaders\fs_fallback.glsl (
		copy /y %1fallback\fs_fallback.glsl %1..\..\core\shaders\ > nul
		echo core/shaders/fs_fallback.glsl
	)
	if not exist %1..\..\core\shaders\vs_input.glsl (
		copy /y %1fallback\vs_input.glsl %1..\..\core\shaders\ > nul
		echo core/shaders/vs_input.glsl
	)
	if not exist %1..\..\core\shaders\vs_model.glsl (
		copy /y %1fallback\vs_model.glsl %1..\..\core\shaders\ > nul
		echo core/shaders/vs_model.glsl
	)
	if not exist %1..\..\core\models mkdir %1..\..\core\models
	if not exist %1..\..\core\models\error.bmdl (
		copy /y %1fallback\error.bmdl %1..\..\core\models\ > nul
		echo core/models/error.bmdl
	)
	if not exist %1..\..\core\models\error.bmat (
		copy /y %1fallback\error.bmat %1..\..\core\models\ > nul
		echo core/models/error.bmat
	)
)