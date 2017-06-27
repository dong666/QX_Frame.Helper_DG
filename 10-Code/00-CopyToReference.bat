@echo off

echo ******************* Copy QX_Frame.Helper_DG_NETFramework461 **************************

xcopy  %cd%"\QX_Frame.Helper_DG_NETFramework461\bin\Debug" %cd%"\QX_Frame.Reference\QX_Frame.Helper_DG_NETFramework461" /y /E /S

echo ******************* Copy QX_Frame.Helper_DG_NETFramework45 **************************

xcopy  %cd%"\QX_Frame.Helper_DG_NETFramework45\bin\Debug" %cd%"\QX_Frame.Reference\QX_Frame.Helper_DG_NETFramework45" /y /E /S

echo ******************* Copy QX_Frame.Helper_DG_NETStandard14 **************************

xcopy  %cd%"\QX_Frame.Helper_DG_NETStandard14\bin\Debug" %cd%"\QX_Frame.Reference\QX_Frame.Helper_DG_NETStandard14" /y /E /S

pause
