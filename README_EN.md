[Chinese Description](https://github.com/zcstkhk/RhinoInside.NX/blob/master/README.md)

# RhinoInside.NX

A Rhino Plug-in For Siemens NX.

# It's now in a very very early beta. for now, it's not recommended for any actual use.

# Note
>1. To install this Plug-in, you must install Rhino 7 and Siemens NX 1847 or higher.
>2. Now the program uses STEP to interact Body data between NX and Rhino, so your NX should has a STEP242 license to use this tool.
>3. There is an issue in NX 1953, when import step data, it will throw an error. so avoid usins NX 1953.

Download latest version [Click here](https://github.com/zcstkhk/RhinoInside.NX/releases/)

The program offered is signed with NX 1872, you can use it in higher version of NX in theoraise an issuery, if not, you can build the program by yourself then sign the program. For more detail about the signing progress, click the link below. Or raise an issue to let me build another specific version for you.

[Signing Process](https://docs.plm.automation.siemens.com/tdoc/nx/1847/nx_api/#uid:signing_process)

# How to use
>1. Install Rhino and NX.
>2. Open RhinoInside.NX Starter.
>3. Select the paths as desired.
>4. Click Start.

# Build Process
>1. Install NX within directory like XXX\Siemens\NXVER, XXX is any folder you have write access. NXVER is the version number of NX, e.g. NX1872, NX1953. Be careful not to have spaces between NX and the version number.
>2. Create Enviroment Variable SPLM_ROOT_DIR, Point to XXX\Siemens in step 1.
>3. Create Enviroment Variable RHINO_ROOT_DIR, Point to RHINO install Path, e.g C:\Programs\Rhino\7.

# Known Issues
>1. Can not rename Grasshopper Component, it will stuck the program.
>2. NX 1953 STEP import problem.