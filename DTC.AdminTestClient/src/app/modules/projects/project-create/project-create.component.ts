import { Component } from '@angular/core';
import { environment } from '../../../../environment/environment';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { CommonModule, DatePipe } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatSelectModule } from '@angular/material/select';
import { MatButton } from '@angular/material/button';
import { AuthorGroup } from '../../../core/models/AuthorGroup';
import { ProjectType } from '../../../core/models/ProjectType';

@Component({
  selector: 'app-project-create',
  standalone: true,
  imports: [CommonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatSelectModule,
    ReactiveFormsModule,
    MatButton],
  templateUrl: './project-create.component.html',
  styleUrl: './project-create.component.css'
})
export class ProjectCreateComponent {
form!: FormGroup;
  baseUrl = environment.apiUrl;
  // файлы
  coverFile: File | null = null;
  selectedFiles: File[] = [];

  isDraggingOverCover = false;
  isDraggingOverFiles = false;

  types: ProjectType[] = [];
  authors: AuthorGroup[] = [];
  isLoading = false;
  error: string | null = null;

  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    private datePipe: DatePipe
  ) { }

  ngOnInit(): void {
    this.form = this.fb.group({
      name: ['', Validators.required],
      version: ['', Validators.required],
      versionDate: ['', Validators.required],
      description: ['', Validators.required],
      isOpenSource: [false],
      authorGroupId: ['', Validators.required],
      projectTypeId: ['', Validators.required],
      beginAge: [0, Validators.required],
      endAge: [0, Validators.required],
      files: [null]
    });

    this.getAllTypes();
    this.getAllCreators();
  }

  onCoverChange(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.coverFile = input.files[0];
    }
  }

  onCoverDrop(event: DragEvent): void {
    event.preventDefault();
    this.isDraggingOverCover = false;
    if (event.dataTransfer?.files && event.dataTransfer.files.length > 0) {
      this.coverFile = event.dataTransfer.files[0];
    }
  }

  removeCover(): void {
    this.coverFile = null;
  }

  onFileChange(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.addFiles(input.files);
    input.value = '';
  }

  onFileDrop(event: DragEvent): void {
    event.preventDefault();
    this.isDraggingOverFiles = false;
    this.addFiles(event.dataTransfer?.files || null);
  }

  onDragOver(event: DragEvent): void {
    event.preventDefault();
    this.isDraggingOverCover = true;
    this.isDraggingOverFiles = true;
  }

  onDragLeave(event: DragEvent): void {
    event.preventDefault();
    this.isDraggingOverCover = false;
    this.isDraggingOverFiles = false;
  }

  addFiles(files: FileList | null): void {
    if (files) {
      Array.from(files).forEach(file => {
        if (!this.selectedFiles.some(f => f.name === file.name && f.size === file.size)) {
          this.selectedFiles.push(file);
        }
      });
    }
  }

  removeFile(index: number): void {
    this.selectedFiles.splice(index, 1);
  }

  // 📤 отправка
  submit(): void {
    if (this.form.valid) {
      const formData = new FormData();

      // текстовые поля
      formData.append('Name', this.form.value.name);
      formData.append('Version', this.form.value.version);

      if (this.form.value.versionDate) {
        const formattedDate = this.datePipe.transform(this.form.value.versionDate, 'yyyy-MM-dd');
        formData.append('VersionDate', formattedDate!);
      }

      formData.append('Description', this.form.value.description);
      formData.append('IsOpenSource', this.form.value.isOpenSource);
      formData.append('AuthorGroupId', this.form.value.authorGroupId);
      formData.append('ProjectTypeId', this.form.value.projectTypeId);
      formData.append('BeginAge', this.form.value.beginAge);
      formData.append('EndAge', this.form.value.endAge);

      // обложка
      if (this.coverFile) {
        formData.append('PhotoFile', this.coverFile, this.coverFile.name);
      }

      // файлы проекта
      this.selectedFiles.forEach(file => {
        formData.append('Files', file, file.name);
      });

      // debug
      for (const pair of formData.entries()) {
        console.log(pair[0], pair[1]);
      }

      this.http.post(`${this.baseUrl}Project`, formData).subscribe({
        next: res => console.log("✅ Успешно:", res),
        error: err => console.error("❌ Ошибка отправки:", err)
      });
    } else {
      console.warn("❌ Форма невалидна");
      this.form.markAllAsTouched();
    }
  }
  getAllTypes(): void {
    this.isLoading = true;
    this.error = null;

    this.http.get<ProjectType[]>(`${this.baseUrl}Project/project-types`)
      .subscribe({
        next: types => {
          this.types = types;
          this.isLoading = false;
        },
        error: err => {
          this.error = 'Ошибка загрузки типов проектов';
          this.isLoading = false;
          console.error('Error loading project types:', err);
        }
      });
  }

  getAllCreators(): void {
    this.http.get<AuthorGroup[]>(`${this.baseUrl}Project/creators`)
      .subscribe({
        next: authors => {
          this.authors = authors;
          this.isLoading = false;
        },
        error: err => {
          this.error = 'Ошибка загрузки авторов';
          this.isLoading = false;
          console.error('Error loading authors:', err);
        }
      });
  }
}
