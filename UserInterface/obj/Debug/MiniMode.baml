   M S B A M L   `   `   `  ����H  DUserInterface, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:UserInterfaceUserInterface  5   	   U QMahApps.Metro, Version=1.5.0.23, Culture=neutral, PublicKeyToken=f4fb5a3c4d1e5b4fZ=clr-namespace:MahApps.Metro.Behaviours;assembly=MahApps.MetroMahApps.Metro.Behaviours 5
          UserInterface.MiniMode   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   -localclr-namespace:UserInterface     5   	   ;Controls,http://metro.mahapps.com/winfx/xaml/controls  5   	   c _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity  5	   	   N
Behaviours=clr-namespace:MahApps.Metro.Behaviours;assembly=MahApps.Metro  5
      -   5          � Title$  MiniMode��5   	   $	��250��6   $	��300��6'   .  (System.Windows.Interactivity.Interaction   	Behaviors 5      7  1MahApps.Metro.Behaviours.BorderlessWindowBehavior  5   
   .��5      � ���#EBDFCF � ColumnDefinitions 5   
   �� 5      $��70 �6   �� 5      .+�5   
   #  MahApps.Metro.Controls.Flyout  -   5          Name$ flyoutOption��5       K� ZIndex$ 10��60   ���#2C6049.��5      �� -   5         ��Name$ 	lb_option��5      
������  �� SelectedIndex$ 0��6F   .@�5      �� $��50��6"   
������ .��5      �� $��En-Vi��6   $��Calibri)�6,   $��20(�6A   ���#EBDFCF6f   5      �� 5      $��50��6   
������ .��5      �� $��En-En��6   $��Calibri)�6,   $��20(�6A   ���#EBDFCF6f   5      �� 5      $��50��6   
������ .��5      �� $��Vi-En��6   $��Calibri)�6,   $��20(�6A   ���#EBDFCF6f   5       5!      5#      �� 5$   
   -   5        $��Option��5$      $	��Top=�6"   $��15q�6:   $��Center=�6[   ��5%      �� 5&      .R�5'      �� ._�5(      �� ��5)      �� 5*   "    �� 
TargetType" ��6(   .�5+   &   ��  �� Property � ��	 �� Value$	 .\Resources\gearfade.png��6?   
 �� Triggers
 5,   &   P� 5-   *    P� Property �@J�IsMouseOver P� Value . . �5.   .   ��  � ��$	 -3q�6G   6T   �� 5/   .    � ��$	 .\Resources\gear.png��6G   50   +   52   #   56      57      58      5:      � 5;   
   $��1��6   
������  � RowDefinitions 5<      �� 5=      �� 5>      .+�5@      �� $��10q�6   
����� ����#2C6049 �� CornerRadius$ 10��6J   6^   �� 5A      -   5         ��Name$ rchtb_input��5A       �� TextWrapping$
 Wrap=�6+   $��Calibri)�6b   $��0q�6w   $��15(�6�   $��Center=�6�   $��Center=�6�   ���#EBDFCF
������ $��15q�6�   ��6?   �� *Translate.KeyWord+6  �� 5B      $��1��6   $��10q�6"   
����� �
�������$ 10��6U   6i   �� 5C      -   5        $ rchtb_output��5C      $��Center=�6V   $��Calibri)�6q   $��15(�6�   ���#2C6049$��Center=�6�   $��1��6�   $��0q�6�   $��15q�6�   ��6,   �� *Translate.ContentKeyWord+6�   5E      5H      5I      