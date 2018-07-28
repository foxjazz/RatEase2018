rm *.txt;
echo "d done"
files = ls *.bmp;
echo "$files"
for filename in ls *.bmp; do
  
  let idx++;
  
  tesseract $filename t$idx -l eng -oem 3 -psm 7
  rm $filename
done

echo "done"
