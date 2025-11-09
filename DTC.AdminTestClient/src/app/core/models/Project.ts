import { Status } from "./Status";

// models/create-project.dto.ts
export interface Project {
  id: number;
  name: string;
  version: string;
  versionDate: string;
  description: string;
  isOpenSource: boolean;
  photoUrl?: string;
  authorGroupId: number;
  status: Status;
  projectTypeId: number;
  beginAge: number;
  endAge: number;
  files: File[];
}
