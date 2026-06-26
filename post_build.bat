if not exist %2%3\glfw3.dll (
	copy /y %1Thirdparty\glfw3.dll %2%3 > nul
	echo glfw3.dll
)
if not exist %2%3\soft_oal.dll (
	copy /y %1Thirdparty\soft_oal.dll %2%3 > nul
	echo soft_oal.dll
)
if not exist %2%3\slangc.exe (
	copy /y %1Thirdparty\slangc.exe %2%3 > nul
	echo slangc.exe
)
if not exist %2%3\slang-compiler.dll (
	copy /y %1Thirdparty\slang-compiler.dll %2%3 > nul
	echo slang-compiler.dll
)
if not exist %2%3\slang-glsl-module.dll (
	copy /y %1Thirdparty\slang-glsl-module.dll %2%3 > nul
	echo slang-glsl-module.dll
)
if not exist %1..\..\core.zip (
	if not exist %1..\..\core mkdir %1..\..\core
	if not exist %1..\..\core\shaders mkdir %1..\..\core\shaders
	if not exist %1..\..\core\shaders\vs_fallback.glsl (
		copy /y %1Fallback\vs_fallback.glsl %1..\..\core\shaders\ > nul
		echo core/shaders/vs_fallback.glsl
	)
	if not exist %1..\..\core\shaders\fs_fallback.glsl (
		copy /y %1Fallback\fs_fallback.glsl %1..\..\core\shaders\ > nul
		echo core/shaders/fs_fallback.glsl
	)
	if not exist %1..\..\core\models mkdir %1..\..\core\models
	if not exist %1..\..\core\models\error.bmdl (
		copy /y %1Fallback\error.bmdl %1..\..\core\models\ > nul
		echo core/models/error.bmdl
	)
	if not exist %1..\..\core\models\error.bmat (
		copy /y %1Fallback\error.bmat %1..\..\core\models\ > nul
		echo core/models/error.bmat
	)
)