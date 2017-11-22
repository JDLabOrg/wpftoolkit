WpfExToolkit for IUEditor
============

A fork of https://wpftoolkit.codeplex.com/

The "Master" Branch will always be synced to the official Codeplex Version.

"iueditor" Branch is customized from "master" branch

---

## History

* 2017-11-22
	* added EditorEnabled dependency property in PropertyGrid for CanChange function

* 2017-10-20
	* property grid clear button image change / border color  
	* wizard button style add (binding) 


* 2017-10-17 : upstream change([https://github.com/xceedsoftware/wpftoolkit](https://github.com/xceedsoftware/wpftoolkit "xceed"))
	* color applied (no color) 
	* numeric (remove - code 위치 표시)  


--------------------
### xceed old github repository (https://github.com/dotnetprojects/WpfExtendedToolkit)

* 2017-10-12 : Extended(upstream) merge
	> color picker (no color 유지, advanced 복귀)

* 2017-07-12 : CommonNumericUpDown Spinner
	> Value가 null일 때, increase/decrease를 하는 경우, DefaultValue가 있으면 DefaultValue를 기준으로 변경

* 2017-06-30 : CommonNumericUpDown Spinner for UpdateValueOnEnterKey
	> UpdateValueOnEnterKey = true 이어도 Spinner로 Update하는 경우에는 Enter key없이 Update 되도록 변경

* 2017-06-22 : ColorPicker design (iueditor customizing)
	> 일단은 ColorPicker/Themes/Aero2.NormarColor.xaml와 ColorPicker/Themes/Generic.xaml 의 코드를 동일하게유지. Theme관련된 부분은 추후에 다시 논의
