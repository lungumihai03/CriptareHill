# Hill Cipher Application

## Overview
This application is a Windows Forms-based tool developed in C# that implements the Hill Cipher, a polygraphic substitution cipher based on linear algebra. It enables users to encrypt and decrypt text using a 3x3 key matrix. The application provides a graphical interface for inputting text and a key matrix, with detailed logging of encryption and decryption steps to files. It is designed for educational or cryptographic exploration purposes.

## Features
- **Encryption and Decryption**: Encrypts and decrypts text using the Hill Cipher with a 3x3 key matrix.
- **Matrix Key Input**: Accepts a 3x3 matrix as the encryption/decryption key, entered via a rich text box.
- **Automatic Text Processing**: Converts input text to uppercase and pads it with 'X' to ensure compatibility with the 3x3 matrix.
- **Inverse Matrix Calculation**: Computes the modular inverse of the key matrix for decryption, ensuring the determinant is invertible modulo 26.
- **Step-by-Step Logging**: Writes detailed encryption and decryption steps to `cript.txt` and `decript.txt` files, respectively.
- **Error Handling**: Validates key matrix input and displays errors for invalid matrices or non-invertible keys.

## Usage
1. **Run the Application**: Launch the application in a Windows environment with the .NET Framework installed.
2. **Enter Key Matrix**: Input a 3x3 matrix in the rich text box, with each row containing three integers separated by spaces or commas (e.g., `1 2 3` on each line).
3. **Input Text**: Enter the text to encrypt or decrypt in the input text box.
4. **Encrypt/Decrypt**:
   - Click the "Encrypt" button to encrypt the input text. The result appears in the output text box, and encryption steps are logged to `cript.txt`.
   - Click the "Decrypt" button to decrypt the text in the output text box (typically the encrypted text). The result updates the output text box, and decryption steps are logged to `decript.txt`.
5. **View Results**: The output label indicates whether the result is encrypted or decrypted. Check `cript.txt` or `decript.txt` for detailed computation steps.
6. **Handle Errors**: Error messages appear via message boxes for issues like invalid matrix formats or non-invertible keys (determinant not coprime with 26).
