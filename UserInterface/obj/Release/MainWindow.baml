   M S B A M L   `   `   `  ���� H  DUserInterface, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:UserInterfaceUserInterface  p lSystem.Windows.Controls.Input.Toolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35pTclr-namespace:System.Windows.Controls;assembly=System.Windows.Controls.Input.ToolkitSystem.Windows.Controls U QMahApps.Metro, Version=1.5.0.23, Culture=neutral, PublicKeyToken=f4fb5a3c4d1e5b4fZ=clr-namespace:MahApps.Metro.Behaviours;assembly=MahApps.MetroMahApps.Metro.Behaviours     UserInterface.MainWindow   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     -localclr-namespace:UserInterface     ;Controls,http://metro.mahapps.com/winfx/xaml/controls  X TXceed.Wpf.Toolkit, Version=3.0.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f44xctk)http://schemas.xceed.com/wpf/xaml/toolkit  btoolkitTclr-namespace:System.Windows.Controls;assembly=System.Windows.Controls.Input.Toolkit  c	 _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity 	 N
Behaviours=clr-namespace:MahApps.Metro.Behaviours;assembly=MahApps.Metro 	   � Title$  
MainWindow��$	��600��$	��800�� � WindowStartupLocation$ CenterScreen=� � AllowsTransparency .  
������ . 	 (System.Windows.Interactivity.Interaction   	Behaviors 7  1MahApps.Metro.Behaviours.BorderlessWindowBehavior  .��=  7MahApps.Metro.Controls.MetroAnimatedSingleRowTabControl   � 	Resources    AccentColorBrush �� Color  HighlightBrush��%9   &	        &	      �� $ #EBDFCF���� $ Orange��.@�)  #MahApps.Metro.Controls.MetroTabItem  $
��Main�����#2C6049.���  � ColumnDefinitions �� �� $��2* �.+�� ���#2C6049 � RowDefinitions �� �� $��8* �.+�� $��1��.+�#  MahApps.Metro.Controls.Flyout  -     Name$ flyoutOption��$��0q����#2C6049$	��250��.��o� 
������ .@�m� 
������ ���� .R�� 
������  �� $��80 ��� .+��� 	 ��Name$	 img_iconfav��$��15q����� 
 �� 
TargetType"
 ��.���  �� Property � �� �� Value$' !.\Resources\essentials-10fade.png���� $��1��$��Calibri)����#EBDFCF$��Center=�$��20(�$��Mark as favorite�� �� Triggers P�  P� Property �@J�IsMouseOver P� Value . . ���  � ��$ -3q���  �� 
TargetName$ img_iconfav�� � ��$# .\Resources\essentials-10.png��m� 
������ ���� .R�� 
������  �� $��80 ��� .+��� $	 img_icontag��$��15q����� "
 ��.���  � ��$# .\Resources\summer-15fade.png���� $��1��$��Calibri)����#EBDFCF$��Center=�$��20(�$��Add tag�� P�  �@J�IsMouseOver . . ���  � ��$ -3q��� $ img_icontag�� � ��$ .\Resources\summer-15.png���� $��0q�$��5,5,5,5q����#EBDFCF �� CornerRadius$ 30���  �� �� $��80 �.+��� -    ��Name$ 
btn_search��$��5,8,5,8q�$��1��$��Center=�$��Center=����� .R��� ._��� $	 
img_search������ "
 ��.���  � ��$# .\Resources\summer-02fade.png�� �� Triggers P�  �@J�IsMouseOver . . ���  � ��$ -3q���  � ��$ .\Resources\summer-02.png��-  'System.Windows.Controls.AutoCompleteBox  -     Name$ txtbx_input��   IsTextCompletionEnabled . $��Calibri)����#2C6049$��	35, 2,2,2q�$��Center=�$��30(�
������ $��0q�� $��15q�$��1�� K� ZIndex$	 -10�����#2C6049 �� $��55 ���  �� $��50 ��� .+��� $��.\Resources\feather.png��$	��Top=�$	��Top=�$��50��$��50���� $��1��$��History:��$��Calibri)����#EBDFCF$��30(��� -    ��Name$ list_history��$��0q�$��1��$��25(�
������ $��2������ "
 ��.���  � ��$ 14(���  � ��
 ���� ��  � �� �#EBDFCF��  � ��$ Italic$� P�  �@J�IsMouseOver . . ���  � �� �#EBDFCF��  � ��$ 0q���  � ��$
 Bold"�� $��1�����#EBDFCF.+��� -    ��Name$ Content��$��0q�
������ $��Calibri)�$��20q� �� 
IsReadOnly .   $��Advanced�����#098A74.��� ���#098A74 �� �� $��8* ��� .+��  �� �� �� ��  �� Orientation$ 
Horizontal=�.+��� $��
    From: ��$��Calibri)����#EBDFCF$��30(�$��Center=��� -    ��Name$ 	cmbx_from��$	��180��$��0q����#EBDFCF$��Medium"�
������ $��30(�$��Center=�� $��1��$��Right=�.+��� $ 
Horizontal=�.+��� $
��To: ��$��Calibri)����#EBDFCF$��30(�$��Center=��� -   $ cmbx_to��$	��180��$��0q����#EBDFCF$��Medium"�
������ $��30(�$��Center=��� $��                    ��� $��1�� �� �� .+��� ���#EBDFCF$��15q�$ 30���� -   $ btn_voiceregco��$ 10��$��30q�$	��Top=�$��Right=����� .R��� ._��� ���� "
 ��.���  � ��$ 40����  � ��$ 40����  � ��$ .\Resources\21fade.png�� P�  �@J�IsMouseOver . . ���  � ��$ .\Resources\21.png���� -	   $ rchtxtbx_input��$��0q�$��20(����#098A74$��Calibri)�$��40q�
������ �� 
������ ���#EBDFCF$��5q�$��1��$��15q�$ 30���� -
   $ rchtxtbx_output��$��1��$��0q�$��20(����#EBDFCF$��Calibri)�$��40q�
������   -   $
��Mini�����#009933.��� ���#009933 �� �� $	��12* �.+��� $��Center=�$ 
Horizontal=�.+��� $��.\Resources\Nezumi.png��$��5q��� $��Center=�$��Calibri)����#EBDFCF$��Italic$�$o��iDouble Click on the tab to open Mini mode, experience fast lookup on every other program's text and image��  $��Setting�����#575A8B.��� ���#575A8B.+��  �� �� $��2* � �� $	��150 ��� .+��� -   $ cbbx_ChooseDictionary��$��0q�$��10q�$��40(�$
��Bold"����#EBDFCF$��1��$
��Left=�$��Center=�
������  �� SelectedIndex$ 0��.@��� $��English - Vietnamese�����#EBDFCF�� $��English - English���� $%��Tiếng Việt - Tiếng Anh���� $��Dictionary:��$��50(����#EBDFCF$��Calibri)�$��Right=�$��Center=�  $��	Favorites�����#D23428.��� ���#D23428 �� �� $	��12* �.+��� $��Center=�$ 
Horizontal=�.+��� $��.\Resources\hato.png��$��10q��� $��Center=�$��Calibri)����#EBDFCF$��Italic$�$����Mark out for interesting vocabulary yourself to improve your active English. They may come in handy for you when you need to write essay, introduction letter or even greeting card!��