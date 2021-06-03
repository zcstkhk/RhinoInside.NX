# RhinoInside.NX
A Rhino Plug-in For Siemens NX.

# 将 NX 中的面转换为 Rhino Brep 的时候遇到一些问题，还请有相关经验的小伙伴不吝赐教。

# It's now in a very very early beta. for now, it's not recommended for any actual use.

# Install Guide
To install this Plug-in, you must install Rhino 7 and Siemens NX 1847 or higher.

Download the latest version [Here](https://github.com/zcstkhk/RhinoInside.NX/releases/)

The program offered is signed with NX 1872, that means if you use 1847 or other version, you must build the program by yourself then sign the program. For more detail about the signing progress, click the link below.

[Signing Process](https://docs.plm.automation.siemens.com/tdoc/nx/1847/nx_api/#uid:signing_process)

Here is the guide for install this plug-in.

>1. Install the Rhino and NX.
>2. Download the Program zip file, then copy the Program folder to any position you like, for example D:\\
>3. Create a enviroment variant, the name is UGII_USER_DIR, the value is where the position that Program folder in. For example, D:\\
>4. Create a enviroment variant, the name is UGII_RhinoInside_Dir, the value is where the position that Program folder in. For example, D:\\
>5. Start NX, you will see a ribbon named RhinoInside.

# 使用方法
>1. 安装 Rhino 和 NX.
>2. 编译或者下载程序包并解压，比如 D:\。
>3. 创建环境变量，名称为 UGII_USER_DIR，值为刚刚解压后的目录，比如 D:\，请注意这个目录下面包含 Application 文件夹，请勿将 Application 也写到环境变量中。
>4. 创建名为 UGII_RhinoInside_Dir，值与刚才设置的 UGII_USER_DIR 相同。
>5. 启动 NX，将会看到 RhinoInside 工作区。