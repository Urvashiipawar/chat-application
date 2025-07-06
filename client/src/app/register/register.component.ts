import { Component, inject } from '@angular/core';
import { AuthServiceService } from '../services/auth-service';
import{MatFormFieldModule} from "@angular/material/form-field"
import { MatInputModule } from '@angular/material/input';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-register',
  imports: [MatFormFieldModule,FormsModule, MatInputModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  email!: string;
  password!: string;
  fullName!: string;
  profilePicture: string = 'https://randomuser.me/api/portraits/lego//5.jpg';
  profileImage: File | null = null;

  authService = inject(AuthServiceService);

  

}
