[English Description](README_EN.MD)

# RhinoInside.NX

A Rhino Plug-in For Siemens NX.

# 目前仍然是处于非常早期的测试版本.

# Install Guide
To install this Plug-in, you must install Rhino 7 and Siemens NX 1847 or higher.

下载最新版本 [点击此处](https://github.com/zcstkhk/RhinoInside.NX/releases/)

The program offered is signed with NX 1872, that means if you use 1847 or other version, you must build the program by yourself then sign the program. For more detail about the signing progress, click the link below.

[签名过程](https://docs.plm.automation.siemens.com/tdoc/nx/1847/nx_api/#uid:signing_process)

# 使用方法
>1. 安装 Rhino 和 NX.
>2. 打开 RhinoInside.NX Starter.
>3. 根据界面提示进行选择.
>4. 点击启动.

# 编译方法
>1. 安装 NX 时将安装目录设置为如下形式，XXX\Siemens\NXVER，其中 XXX 为任意文件夹，注意最好不要有空格，否则可能会引发其它问题，NXVER 为 NX 的版本号，比如 NX1872、NX1953，注意 NX 和 版本号之间不要有空格。
>2. 创建环境变量 SPLM_ROOT_DIR，指向第一步安装路径中的 XXX\Siemens。
>3. 创建环境变量 RHINO_ROOT_DIR，指向 RHINO 的安装目录，比如 C:\Programs\Rhino\7。

#已知问题
>1. 不能修改 Grasshopper 中组件的描述（名称），否则会卡死，原因暂时未知。
>2. 无法正确创建被平面修剪后断面为椭圆的圆锥/圆锥面，提示 ON_Brep.m_T[1] 2d curve is not inside surface domain。