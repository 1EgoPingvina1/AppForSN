export interface User {
  id:number,
  username: string;
  token: string;
  refreshToken?: string;
  tokenExpiration?: number;
  photoUrl: string;
  roles: string[];
  firstName: string;
  secondName?: string;
  lastName: string;
  gender: string;
  isAuthor: boolean;
  birthday: string;
}