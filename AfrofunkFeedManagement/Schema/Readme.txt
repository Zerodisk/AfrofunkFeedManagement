***** This is a procedure to re-produce the xsd and c# class *****

1. convert xml to xsd using http://www.flame-ware.com/products/xml-2-xsd/default.aspx
2. got xsd file
3. run  "xsd /c /f /l:cs /namespace:Schema Iconic.xsd"