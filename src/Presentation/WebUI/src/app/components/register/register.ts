import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-register',
  standalone: false,
  templateUrl: './register.html',
  styleUrl: './register.scss',
})
export class Register {
 
   registerForm: FormGroup;

   constructor(private fb: FormBuilder) {
     this.registerForm = this.fb.group({
       firstName: ['',Validators.required],
       lastName: ['',Validators.required],
       email: ['',Validators.required],
       phoneNumber: ['',Validators.required],
       password: ['',Validators.required],
        confirmPassword: ['',Validators.required],
     },
     {
        validators: this.passwordMatchValidator
     }
    );
   }
   passwordMatchValidator(form: FormGroup) {
    const password = form.get('password');
    const confirmPassword = form.get('confirmPassword');
    
    if (password && confirmPassword && password.value !== confirmPassword.value) {
      confirmPassword.setErrors({ passwordMismatch: true });
      return { passwordMismatch: true };
    }
    return null;
  }

}
