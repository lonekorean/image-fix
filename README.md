# image-fix

Simple .NET utility to scale and crop an image to create a preview/thumbnail.

### Usage

This project provides a single class, `CodersBlock.ImageFix.ImageFixer`, which provides a single static function, `ResizeImage()`. This function takes the following parameters:

- `byte[] imageFile` - The image as a byte array
- `int newWidth` - Width to scale and/or crop the image to
- `int newHeight` -  Height to scale and/or crop the image to
- `long quality` - The quality of the resulting JPG (100 is the max)

`ResizeImage()` returns a byte array for a new resized image that preserves the original image's aspect ratio.

Feel free to incorporate the source code, or just build and use the DLL.
